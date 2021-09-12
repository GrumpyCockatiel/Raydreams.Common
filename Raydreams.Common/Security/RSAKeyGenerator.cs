using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Raydreams.Common.Security
{




    ///// <summary>Generates a new RSA public/private key pair in XML format to use with Asymmetric Encryptor</summary>
    ///// <remarks>Needs a lot of work but its a starting point</remarks>
    //public class RSAKeyGenerator
    //{
    //    /// <summary>Public key stored as XML</summary>
    //    public string PublicKey { get; set; }

    //    /// <summary>Private Key stored as XML</summary>
    //    public string PrivateKey { get; set; }

    //    //public void Load()
    //    //{
    //    //    RSA x = RSA.Create();
    //    //    using var provider = new RSACryptoServiceProvider( 8 );
    //    //    provider.ImportPkcs8PrivateKey
    //    //}

    //    /// <summary>Generate a pair of keys and saves the as XML then BASE64 encoded</summary>
    //    /// <param name="keySize">A valid RSA key size</param>
    //    /// <param name="withKeySize">Add the key size as metadata in front of the key as XML string</param>
    //    /// <returns>key size</returns>
    //    public int GenerateKeys(RSAKeySize keySize, bool withKeySize = true)
    //    {
    //        int ks = (int)keySize;

    //        if (ks % 2 != 0 || ks < 512)
    //            throw new System.Exception("Key should be multiple of two and greater than 512.");

    //        using (var provider = new RSACryptoServiceProvider(ks))
    //        {
    //            // export parameters is probably a better choice over XML - then we can store the parts as desired
    //            RSAParameters sk = provider.ExportParameters( true );

    //            // public key only contain the Modulus and Exponent - everything is bytes
    //            RSAParameters pk = provider.ExportParameters( false );

    //            // returns everything as JSON
    //            this.PublicKey = provider.ToXmlString(false);
    //            this.PrivateKey = provider.ToXmlString(true);

    //            // the key size is prefixed in front of the XML before it is all BASE64 encoded
    //            // This is dumb since it changes the stored value from XML to BASE64.
    //            if ( withKeySize )
    //            {
    //                this.PublicKey = Convert.ToBase64String( Encoding.UTF8.GetBytes( $"{ks}!{this.PublicKey}" ) );
    //                this.PrivateKey = Convert.ToBase64String( Encoding.UTF8.GetBytes( $"{ks}!{this.PrivateKey}" ) );
    //            }
    //        }

    //        return ks;
    //    }

    //    /// <summary>Unwinds a key stored with its key size into a tuple of key size/XML key</summary>
    //    /// <param name="rawkey">raw XML with or without key size prefix</param>
    //    /// <returns></returns>
    //    public static (int KeySize, string XMLKey) GetKey(string rawkey)
    //    {
    //        int keySize = 0;
    //        string xmlKey = String.Empty;

    //        if (rawkey != null && rawkey.Length > 0)
    //        {
    //            byte[] keyBytes = Convert.FromBase64String( rawkey );
    //            string stringKey = Encoding.UTF8.GetString( keyBytes );

    //            if (stringKey.Contains( "!" ))
    //            {
    //                string[] values = stringKey.Split( new char[] { '!' }, 2 );

    //                keySize = Int32.Parse( values[0] );
    //                xmlKey = values[1];
    //            }
    //            else
    //                xmlKey = stringKey;
    //        }

    //        return (keySize, xmlKey);
    //    }

    //}
}
