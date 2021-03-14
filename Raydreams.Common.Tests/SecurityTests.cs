using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raydreams.Common.IO;
using Raydreams.Common.Logic;
using Raydreams.Common.Security;

namespace Raydreams.Common.Tests
{
    [TestClass]
    public class SecurityTests
    {
        private static readonly byte[] key = new byte[] { 0x55, 0x70, 0xf9, 0xd9, 0xb3, 0xd8, 0x0a, 0x7e, 0x2f, 0x59, 0x1a, 0x80, 0x32, 0x06, 0x55, 0xb1, 0x55, 0x70, 0x08, 0xd9, 0xb3, 0xd8, 0x0a, 0x7e, 0x2f, 0x59, 0x1a, 0x80, 0x32, 0x06, 0x55, 0xb2 };

        [TestMethod]
        public void EncryptFileTest()
        {
            string path = $"{IOHelpers.DesktopPath}/test.jpeg";

            // get a handle to the file
            FileInfo fi = new FileInfo( path );
            if ( !fi.Exists )
            {
                Assert.Fail( "No file to test" );
                return;
            }

            // base file name
            string name = Path.GetFileNameWithoutExtension( fi.Name );
            string ext = fi.Extension.TrimStart(new char[] {'.'} );

            // 4.2 Gig limit or throws an exception
            byte[] data = File.ReadAllBytes( path );

            // encrypt
            AESEncryptor enc = new AESEncryptor();
            CipherMessage results = enc.Encrypt( data, key );

            // write to file - never overwrite
            using FileStream fs = new FileStream( $"{IOHelpers.DesktopPath}/{name}.rayx", FileMode.CreateNew, FileAccess.Write);

            // 4 bytes - write a magic number - which is 'ray' followed by 0
            fs.Write( new byte[] { 0x72, 0x61, 0x79, 0x00 } );

            // 2 bytes - write the file format version which is 1.0
            fs.Write( new byte[] { 0x01, 0x00 } );

            // 16 bytes - first write the IV out which is 16 bytes
            fs.Write( results.IV, 0, results.IV.Length );

            // 2 bytes - write a delimiator which is 01
            fs.Write( new byte[] { 0x00, 0x01 } );

            // write the original extension which is 1+len
            byte[] eb = Encoding.UTF8.GetBytes( ext );
            byte[] ebl = BitConverter.GetBytes( eb.Length );
            fs.WriteByte(ebl[0]);
            fs.Write( eb );

            // 2 bytes - write a delimiator which is 01
            fs.Write( new byte[] { 0x00, 0x01 } );

            // write the encrypted data
            fs.Write( results.CipherBytes, 0, results.CipherBytes.Length );

            // flush and close
            fs.Flush();
            fs.Close();

            Assert.IsTrue( results.CipherBytes.Length > 0 );
        }

        [TestMethod]
        public void DecryptFileTest()
        {
            string path = $"{IOHelpers.DesktopPath}/test.rayx";

            // get a handle to the file
            FileInfo fi = new FileInfo( path );
            if ( !fi.Exists )
            {
                Assert.Fail( "No file to test" );
                return;
            }

            // base file name
            string name = Path.GetFileNameWithoutExtension( fi.Name );

            // write to file - never overwrite
            using FileStream fs = new FileStream( path, FileMode.Open, FileAccess.Read );

            // 4 bytes - write a magic number - which is 'ray' followed by 0
            byte[] magic = new byte[4];
            fs.Read( magic , 0, 4 );

            // 2 bytes - write the file format version which is 1.0
            byte[] ver = new byte[2];
            fs.Read( ver );

            // 16 bytes - first write the IV out which is 16 bytes
            byte[] iv = new byte[16];
            fs.Read( iv );

            // 2 bytes - read a delimiator which is 01
            byte[] delim = new byte[2];
            fs.Read( delim );

            // 1 byte - the length of the extension string
            byte[] ebl = new byte[1];
            fs.Read( ebl );
            int el = Convert.ToInt32( ebl[0] );

            // read N bytes the original extension
            byte[] eb = new byte[el];
            fs.Read( eb );
            string ext = Encoding.UTF8.GetString( eb );

            // 2 bytes - read a delimiator which is 01
            fs.Read( delim );

            // finally get the data itself
            int offset = 27 + el;
            byte[] data = new byte[fs.Length - offset];
            fs.Read( data );

            // decrypt
            AESEncryptor enc = new AESEncryptor();
            byte[] file = enc.Decrypt( data, key, iv );

            File.WriteAllBytes( $"{IOHelpers.DesktopPath}/{name}-copy.{ext}", file );

            fs.Close();

            Assert.IsTrue( file.Length > 0 );
        }

        [TestMethod]
        public void MakeKeysTest()
        {
            string path = $"{IOHelpers.DesktopPath}/publickey.asc";
            string[] lines = File.ReadAllLines( path );

            StringBuilder sb = new StringBuilder();

            int i = 0;
            for ( ; i < lines.Length; ++i )
            {
                // fine the start of the public key
                if ( !String.IsNullOrWhiteSpace( lines[i] ) )
                    continue;
                else
                {
                    ++i;
                    break;
                }
            }

            for ( ;  i < lines.Length; ++i )
            {
                if ( lines[i][0] == '=' )
                    break;

                sb.Append( lines[i] );
            }

            byte[] modulus = Convert.FromBase64String( sb.ToString() );
            byte[] exp = Convert.FromBase64String( lines[i].TrimStart('=') );

            RSAParameters pk = new RSAParameters()
            {
                Modulus = modulus,
                Exponent = exp,
            };

            using var provider = new RSACryptoServiceProvider( 512 );
            provider.ImportParameters( pk );

            RSAKeyGenerator gen = new RSAKeyGenerator();
            int size = gen.GenerateKeys( RSAKeySize.Key2048 );

            Assert.IsTrue( true );
        }

        [TestMethod]
        public void NullTokenTest()
        {
            ITokenManager mgr = new NullTokenManager();

            string token = mgr.Encode( null );

            TokenPayload p = mgr.Decode( token );

            Assert.IsTrue( p != null && p.ID == "1" );
        }

        [TestMethod]
        public void SignTest()
        {
            RSAKeyGenerator gen = new RSAKeyGenerator();
            int size = gen.GenerateKeys( RSAKeySize.Key2048, false );

            var data = new byte[] { 0x00, 0x34, 0x56, 0x19 };

            byte[] sig = AsymmetricEncryptor.SignWithRSA256( data, gen.PrivateKey );

            bool verified = AsymmetricEncryptor.VerifyRSA256( data, sig, gen.PublicKey );

            Assert.IsTrue(verified);
        }
    }
}
