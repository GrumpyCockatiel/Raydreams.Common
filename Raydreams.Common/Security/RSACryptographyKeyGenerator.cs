using System;
using System.Security.Cryptography;
using System.Text;

namespace Raydreams.Common.Security
{
    /// <summary>Enumerate possible key sizes</summary>
    public enum RSAKeySize
    { 
        Key512 = 512, 
        Key1024 = 1024,
        Key2048 = 2048,
        Key4096 = 4096
    }

    /// <summary>Generates a new RSA public/private key pair in XML format to use with Asymmetric Encryptor</summary>
    public class RSAKeyGenerator
    {
        /// <summary>Public key created</summary>
        public string PublicKey { get; set; }

        /// <summary>Private Key Created</summary>
        public string PrivateKey { get; set; }

        /// <summary>Generate a pair of keys</summary>
        /// <param name="keySize"></param>
        /// <param name="withKeySize">Prefix with the key size in front of the XML string</param>
        /// <returns>key size</returns>
        public int GenerateKeys(RSAKeySize keySize, bool withKeySize = true)
        {
            int ks = (int)keySize;

            if (ks % 2 != 0 || ks < 512)
                throw new System.Exception("Key should be multiple of two and greater than 512.");

            using (var provider = new RSACryptoServiceProvider(ks))
            {
                this.PublicKey = provider.ToXmlString(false);
                this.PrivateKey = provider.ToXmlString(true);

                if ( withKeySize )
                {
                    this.PublicKey = Convert.ToBase64String( Encoding.UTF8.GetBytes( ks.ToString() + "!" + this.PublicKey ) );
                    this.PrivateKey = Convert.ToBase64String( Encoding.UTF8.GetBytes( ks.ToString() + "!" + this.PrivateKey ) );
                }
            }

            return ks;
        }

        /// <summary>Unwinds a key stored with its key size into a tuple of key size/XML key</summary>
        /// <param name="rawkey">raw XML with key size prefix</param>
        /// <returns></returns>
        public static (int KeySize, string XMLKey) GetKey(string rawkey)
        {
            int keySize = 0;
            string xmlKey = String.Empty;

            if (rawkey != null && rawkey.Length > 0)
            {
                byte[] keyBytes = Convert.FromBase64String( rawkey );
                string stringKey = Encoding.UTF8.GetString( keyBytes );

                if (stringKey.Contains( "!" ))
                {
                    string[] values = stringKey.Split( new char[] { '!' }, 2 );

                    keySize = Int32.Parse( values[0] );
                    xmlKey = values[1];
                }
                else
                    xmlKey = stringKey;
            }

            return (keySize, xmlKey);
        }


    }
}
