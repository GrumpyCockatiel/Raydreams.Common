using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

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

    /// <summary>Class for de/serializing key values into JSON</summary>
    /// <remarks>These are stored as BASE64 strings though you could keep them as byte data depending on your storage</remarks>
    public class RSAKeyValues
    {
        public RSAKeyValues(){ }

        public RSAKeyValues( RSAParameters key )
        {
            this.Modulus = Convert.ToBase64String( key.Modulus );
            this.Exponent = Convert.ToBase64String( key.Exponent );

            if ( key.P != null && key.P.Length > 0 )
            {
                this.P = Convert.ToBase64String( key.P );
                this.Q = Convert.ToBase64String( key.Q );
                this.DP = Convert.ToBase64String( key.DP );
                this.DQ = Convert.ToBase64String( key.DQ );
                this.InverseQ = Convert.ToBase64String( key.InverseQ );
                this.D = Convert.ToBase64String( key.D );
            }
        }

        /// <summary>Strips private values from the key so it can be used as a public key</summary>
        /// <returns></returns>
        public RSAKeyValues MakePublic()
        {
            return new RSAKeyValues() { Modulus = this.Modulus, Exponent = this.Exponent };
        }

        /// <summary>Is this a private key</summary>
        public bool IsPrivate => ( this.P != null && this.P.Length > 0 );

        /// <summary>From the BASE64 rep back into the byte representation</summary>
        public RSAParameters Parameters
        {
            get
            {
                RSAParameters results = new RSAParameters
                {
                    Modulus = Convert.FromBase64String( this.Modulus ),
                    Exponent = Convert.FromBase64String( this.Exponent )
                };

                if ( this.IsPrivate )
                {
                    results.P = Convert.FromBase64String( this.P );
                    results.Q = Convert.FromBase64String( this.Q );
                    results.DP = Convert.FromBase64String( this.DP );
                    results.DQ = Convert.FromBase64String( this.DQ );
                    results.InverseQ = Convert.FromBase64String( this.InverseQ );
                    results.D = Convert.FromBase64String( this.D );
                }

                return results;
            }
        }

        [JsonProperty( "m" )]
        public string Modulus { get; set; }

        [JsonProperty( "exp" )]
        public string Exponent { get; set; }

        [JsonProperty( "p" )]
        public string P { get; set; }

        [JsonProperty( "q" )]
        public string Q { get; set; }

        [JsonProperty( "dp" )]
        public string DP { get; set; }

        [JsonProperty( "dq" )]
        public string DQ { get; set; }

        [JsonProperty( "iq" )]
        public string InverseQ { get; set; }

        [JsonProperty( "d" )]
        public string D { get; set; }

        [JsonProperty( "size" )]
        public RSAKeySize KeySize { get; set; }
    }

    /// <summary>Methods for handling asym encryption using RSA keys</summary>
    /// <remarks>If you stick with the enum key size there should not be key size issues, checks are left for when it was an int</remarks>
    public static class AsymmetricEncryptor
    {
        /// <summary>Makes a new set of keys and returns them as BASE64 encoded</summary>
        /// <param name="keySize"></param>
        /// <returns></returns>
        public static (RSAKeyValues pk, RSAKeyValues sk) MakeKeys( RSAKeySize keySize )
        {
            int ks = (int)keySize;

            if ( ks % 2 != 0 || ks < 512 )
                throw new System.Exception( "Key should be multiple of two and greater than 512." );

            using var provider = new RSACryptoServiceProvider( ks );

            // secret key
            RSAParameters sk = provider.ExportParameters( true );

            // public key
            RSAParameters pk = provider.ExportParameters( false );

            return ( new RSAKeyValues( pk ) { KeySize = keySize }, new RSAKeyValues( sk ) { KeySize = keySize });
        }

        /// <summary>Use padding</summary>
        public static bool OptimalAsymmetricEncryptionPadding = false;

        /// <summary>Signs some data with the specified private asym key using SHA-256</summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] SignWithSHA256( byte[] data, RSAKeyValues key)
        {
            if ( data == null || data.Length < 1 )
                throw new ArgumentException( "Nothing to sign.", "data" );

            // validate input
            if ( !IsKeySizeValid( (int)key.KeySize ) )
                throw new ArgumentException( "Key size is not valid", "keySize" );

            using var provider = new RSACryptoServiceProvider( (int)key.KeySize );
            provider.ImportParameters( key.Parameters );
            byte[] sig = provider.SignData( data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1 );

            return sig;
        }

        /// <summary>Verifies a signature using the corresponding public key</summary>
        /// <param name="data"></param>
        /// <param name="sig"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool VerifyWithSHA256( byte[] data, byte[] sig, RSAKeyValues key )
        {
            if ( data == null || data.Length < 1 )
                throw new ArgumentException( "Nothing to verify.", "data" );

            if ( sig == null || sig.Length < 1 )
                throw new ArgumentException( "No signature", "sig" );

            // validate input
            if ( !IsKeySizeValid( (int)key.KeySize ) )
                throw new ArgumentException( "Key size is not valid", "keySize" );

            using var provider = new RSACryptoServiceProvider( (int)key.KeySize );
            provider.ImportParameters( key.Parameters );
            return provider.VerifyData( data, sig, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1 );
        }

        /// <summary>Encrypt text using a public key</summary>
        public static string Encrypt(string plainText, RSAKeyValues key )
        {
            // validate input
            if ( !IsKeySizeValid( (int)key.KeySize ) )
                throw new ArgumentException( "Key size is not valid", "keySize" );

            if ( String.IsNullOrEmpty(plainText) )
                throw new ArgumentException( "Nothing to encrypt", "plainText" );

            int maxLength = GetMaxDataLength( (int)key.KeySize  );

            byte[] data = Encoding.UTF8.GetBytes( plainText );

            if (data.Length > maxLength)
                throw new ArgumentException( $"Maximum data length is {maxLength}.", "data" );

            using var provider = new RSACryptoServiceProvider( (int)key.KeySize );
            provider.ImportParameters( key.Parameters );
            byte[] encdata = provider.Encrypt( data, OptimalAsymmetricEncryptionPadding );

            return Convert.ToBase64String( encdata );
        }

        /// <summary>Decrypt using the private key</summary>
        public static string Decrypt(string encryptedText, RSAKeyValues key )
        {
            // validate input
            if ( !IsKeySizeValid( (int)key.KeySize ) )
                throw new ArgumentException( "Key size is not valid", "keySize" );

            byte[] encdata = Convert.FromBase64String( encryptedText );

            using var provider = new RSACryptoServiceProvider( (int)key.KeySize );
            provider.ImportParameters( key.Parameters );
            byte[] data = provider.Decrypt( encdata, OptimalAsymmetricEncryptionPadding );
            
            return Encoding.UTF8.GetString( data );
        }

        /// <summary>Returns the maximum allowed amount of data for the specified key size</summary>
        private static int GetMaxDataLength(int keySize)
        {
            if ( OptimalAsymmetricEncryptionPadding )
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
