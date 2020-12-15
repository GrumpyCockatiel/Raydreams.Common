using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Raydreams.Common.Security
{
	/// <summary>Symmetric encryption algorithms</summary>
	public enum SymmetricAlgoType : byte
	{
		/// <summary>AES (aka Rijndael) encryption.</summary>
		AES,
		/// <summary>DES encryption - very weak - use TripleDES at least.</summary>
		DES,
		/// <summary>Triple DES encryption - does DES three times.</summary>
		TripleDES,
		/// <summary>RC2 encryption.</summary>
		RC2
	}

	/// <summary>Struct to contain the message, key and init vector of a symmetric encryption when the key and IV are chosen randomly.</summary>
	/// <remarks>Use Convert.FromBase64String and Convert.ToBase64String to save the bytes as string data or hard code the bytes.
	/// </remarks>
	public struct CipherMessage
	{
		/// <summary></summary>
		public byte[] CipherBytes { get; set; }

		/// <summary>the original key bytes</summary>
		public byte[] Key { get; set; }

		/// <summary>the initialization vector</summary>
		public byte[] IV { get; set; }
	}

	/// <summary>Encrypt and decrypt strings using symmetric algorithms</summary>
	public class SymmetricEncryptor
	{
		#region [ Fields ]

		/// <summary>Algorithm being used</summary>
		private SymmetricAlgorithm _algo = null;

		#endregion [ Fields ]

		#region [ Constructors ]

		/// <summary>Constructor</summary>
		public SymmetricEncryptor( SymmetricAlgoType algo )
		{
			this.CreateEncryptor( algo );
		}

		/// <summary>Selectes the correct algorithm to instantiate</summary>
		/// <param name="type">The algorithm to use.</param>
		private void CreateEncryptor( SymmetricAlgoType type )
		{
			if ( type == SymmetricAlgoType.AES )
				this._algo = new RijndaelManaged();
			else if ( type == SymmetricAlgoType.TripleDES )
				this._algo = new TripleDESCryptoServiceProvider();
			else if ( type == SymmetricAlgoType.RC2 )
				this._algo = new RC2CryptoServiceProvider();
			else
				this._algo = new DESCryptoServiceProvider();
		}

		#endregion [ Constructors ]

		/// <summary>Get the legal key sizes of the instantiated algorithm.</summary>
		public KeySizes KeySizes
		{
			get { return this._algo.LegalKeySizes[0]; }
		}

		/// <summary>Generate an IV for the in use algorithm</summary>
		public byte[] CreateIV()
		{
			this._algo.GenerateIV();
			return this._algo.IV;
		}

		/// <summary>Encrypt a string using a symmetric key algorithm where the key and IV are randomly generated.</summary>
		/// <param name="plainText">The string to encrypt</param>
		/// <param name="keySize">The key  size to use</param>
		public CipherMessage Encrypt( string plainText, int keySize )
		{
			if ( String.IsNullOrWhiteSpace( plainText ) )
				throw new ArgumentNullException( nameof( plainText ) );

			// validate and set the key size
			try
			{
				this._algo.KeySize = keySize;
			}
			catch ( CryptographicException )
			{
				this._algo.KeySize = this._algo.LegalKeySizes[0].MinSize;
			}

			// make a IV and key
			this._algo.GenerateIV();
			this._algo.GenerateKey();

			CipherMessage msg = new CipherMessage();
			msg.IV = this._algo.IV;
			msg.Key = this._algo.Key;

			msg.CipherBytes = this.Encrypt( plainText, this._algo.Key, this._algo.IV );

			return msg;
		}

		/// <summary>Encrypt plain text with a known key</summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <returns>Encrypted bytes and the IV generated</returns>
		public CipherMessage Encrypt( string plainText, byte[] key )
        {
			// validate arguments
			if ( String.IsNullOrWhiteSpace( plainText ) )
				throw new System.ArgumentNullException( nameof( plainText ) );

			if ( key == null || key.Length < 1 )
				throw new System.ArgumentNullException( nameof( key ) );

			CipherMessage results = new CipherMessage();

			// generate an IV
			this._algo.GenerateIV();
			results.IV = this._algo.IV;
			results.Key = key;

			results.CipherBytes = this.Encrypt( plainText, results.Key, results.IV );

			return results;
		}

		/// <summary>Encrypt a string using a symmetric key algorithm.</summary>
		/// <param name="plainText">text to be encrypted</param>
		/// <param name="key">Private symmetric key used for encryption.</param>
		/// <param name="iv">Init vector used offsetting blocks.</param>
		/// <returns></returns>
		public byte[] Encrypt( string plainText, byte[] key, byte[] iv )
		{
			// validate arguments
			if ( String.IsNullOrWhiteSpace( plainText ) )
				throw new ArgumentNullException( nameof(plainText) );
			if ( key == null || key.Length < 1 )
				throw new ArgumentNullException( nameof(key) );
			if ( iv == null || iv.Length < 1 )
				throw new ArgumentNullException( nameof(iv) );

			// Declare the streams used to encrypt to an in memory array of bytes.
			MemoryStream msEncrypt = null;
			CryptoStream csEncrypt = null;
			StreamWriter swEncrypt = null;

			try
			{
				this._algo.Key = key;
				this._algo.IV = iv;

				// Create a decrytor to perform the stream transform.
				ICryptoTransform encryptor = this._algo.CreateEncryptor( this._algo.Key, this._algo.IV );

				// Create the streams used for encryption.
				msEncrypt = new MemoryStream();
				csEncrypt = new CryptoStream( msEncrypt, encryptor, CryptoStreamMode.Write );
				swEncrypt = new StreamWriter( csEncrypt );

				// Write all data to the stream.
				swEncrypt.Write( plainText );

			}
			catch ( System.Exception exp )
			{
				throw exp;
			}
			finally
			{
				// Close the streams.
				if ( swEncrypt != null )
					swEncrypt.Close();
				if ( csEncrypt != null )
					csEncrypt.Close();
				if ( msEncrypt != null )
					msEncrypt.Close();
			}

			// Return the encrypted bytes from the memory stream.
			return msEncrypt.ToArray();
		}

		/// <summary>Decrypt cipter text using a symmetric key algorithm.</summary>
		public string Decrypt( CipherMessage msg )
		{
			return this.Decrypt( msg.CipherBytes, msg.Key, msg.IV );
		}

		/// <summary>Decrypt cipter text using a symmetric key algorithm.</summary>
		/// <param name="cipherText">The encrypted message as a byte array.</param>
		/// <param name="key">The key used in the original encryption.</param>
		/// <param name="iv">The init vector used in the original encryption.</param>
		/// <returns></returns>
		public string Decrypt( byte[] cipherText, byte[] key, byte[] iv )
		{
			// Check arguments.
			if ( cipherText == null || cipherText.Length <= 0 )
				throw new ArgumentNullException( nameof(cipherText) );
			if ( key == null || key.Length <= 0 )
				throw new ArgumentNullException( nameof(key) );
			if ( iv == null || iv.Length <= 0 )
				throw new ArgumentNullException( nameof(iv) );

			// Declare the streams used to decrypt to an in memory array of bytes.
			MemoryStream msDecrypt = null;
			CryptoStream csDecrypt = null;
			StreamReader srDecrypt = null;

			// Declare the string used to hold the decrypted text.
			string plaintext = null;

			try
			{
				this._algo.Key = key;
				this._algo.IV = iv;

				// Create a decrytor to perform the stream transform.
				ICryptoTransform decryptor = this._algo.CreateDecryptor( this._algo.Key, this._algo.IV );

				// Create the streams used for decryption.
				msDecrypt = new MemoryStream( cipherText );
				csDecrypt = new CryptoStream( msDecrypt, decryptor, CryptoStreamMode.Read );
				srDecrypt = new StreamReader( csDecrypt );

				// Read the decrypted bytes from the decrypting stream and place them in a string.
				plaintext = srDecrypt.ReadToEnd();

			}
			catch (System.Exception exp )
			{
				throw exp;
			}
			finally
			{
				// Close the streams.
				if ( srDecrypt != null )
					srDecrypt.Close();
				if ( csDecrypt != null )
					csDecrypt.Close();
				if ( msDecrypt != null )
					msDecrypt.Close();
			}

			return plaintext;
		}

		/// <summary>Zeros out memory.</summary>
		public void Clear()
		{
			// Clear the RijndaelManaged object.
			if ( this._algo != null )
				this._algo.Clear();
		}
	}
}