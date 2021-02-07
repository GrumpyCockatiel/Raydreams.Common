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
