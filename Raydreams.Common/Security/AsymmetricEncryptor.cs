using System;
using System.Security.Cryptography;
using System.Text;

namespace Raydreams.Common.Security
{
    /// <summary>Methods for handling asym encryption using RSA keys</summary>
    public static class AsymmetricEncryptor
    {
        /// <summary>Use padding</summary>
        public static bool OptimalAsymmetricEncryptionPadding = false;

        /// <summary>Signs a byte array with the specified private key</summary>
        /// <param name="data"></param>
        /// <param name="keySize"></param>
        /// <param name="publicXMLKey">The key pair in XML format</param>
        /// <returns></returns>
        public static byte[] SignWithRSA256( byte[] data, int keySize, string publicAndPrivateKeyXml )
        {
            if ( data == null || data.Length < 1 )
                throw new ArgumentException( "Nothing to sign.", "data" );

            // validate input
            if ( !IsKeySizeValid( keySize ) )
                throw new ArgumentException( "Key size is not valid", "keySize" );

            using var provider = new RSACryptoServiceProvider( keySize );
            provider.FromXmlString( publicAndPrivateKeyXml );
            byte[] sig = provider.SignData( data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1 );

            return sig;
        }

        /// <summary>Encrypt text using a piblic key</summary>
        /// <param name="publicXMLKey">The public and private keys in XML format</param>
        public static string Encrypt(string plainText, int keySize, string publicXMLKey)
        {
            // validate input
            if ( !IsKeySizeValid( keySize ))
                throw new ArgumentException( "Key size is not valid", "keySize" );

            if ( String.IsNullOrEmpty(plainText) )
                throw new ArgumentException( "Nothing to encrypt", "plainText" );

            int maxLength = GetMaxDataLength( keySize );

            byte[] data = Encoding.UTF8.GetBytes( plainText );
            byte[] encdata;

            if (data.Length > maxLength)
                throw new ArgumentException( $"Maximum data length is {maxLength}.", "data" );

            using (var provider = new RSACryptoServiceProvider( keySize ))
            {
                provider.FromXmlString( publicXMLKey );
                encdata = provider.Encrypt( data, OptimalAsymmetricEncryptionPadding );
            }

            return Convert.ToBase64String( encdata );
        }

        /// <summary>Decrypt using the private key</summary>
        public static string Decrypt(string encryptedText, int keySize, string publicAndPrivateKeyXml)
        {
            // validate input
            if (!IsKeySizeValid( keySize ))
                throw new ArgumentException( "Key size is not valid", "keySize" );

            byte[] encdata = Convert.FromBase64String( encryptedText );
            byte[] data;

            using (var provider = new RSACryptoServiceProvider( keySize ))
            {
                provider.FromXmlString( publicAndPrivateKeyXml );
                data = provider.Decrypt( encdata, OptimalAsymmetricEncryptionPadding );
            }

            return Encoding.UTF8.GetString( data );
        }

        /// <summary>Returns the maximum allowed amount of data for the specified key size</summary>
        private static int GetMaxDataLength(int keySize)
        {
            if (OptimalAsymmetricEncryptionPadding)
                return ( ( keySize - 384 ) / 8 ) + 7;

            return ( ( keySize - 384 ) / 8 ) + 37;
        }

        /// <summary>Test the keysize is correct value</summary>
        private static bool IsKeySizeValid(int keySize)
        {
            return keySize >= 384 && keySize <= 16384 && keySize % 8 == 0;
        }

    }
}
