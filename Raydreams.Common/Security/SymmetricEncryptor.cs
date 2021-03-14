using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Raydreams.Common.Security
{
	/// <summary>Enumerate the standard encryption algorithm types</summary>
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
	/// <remarks>
    /// The class is really old an in new of an upate but still works as intended.
	/// </remarks>
	public struct CipherMessage
	{
		/// <summary>The encrypted bytes</summary>
		public byte[] CipherBytes { get; set; }

		/// <summary>the original key bytes</summary>
		public byte[] Key { get; set; }

		/// <summary>the initialization vector</summary>
		public byte[] IV { get; set; }
	}

	/// <summary>Encrypt and decrypt strings using symmetric algorithms</summary>
	/// <remarks>
	/// This was written back when people still used DES and TripleDES regularly.
	/// To be deprecated for JUST AES
	/// For AES, the legal key sizes are 128, 192, and 256 bits.
	/// </remarks>
	public class SymmetricEncryptor
	{
		#region [ Fields ]

		/// <summary>Algorithm being used</summary>
		private SymmetricAlgorithm _algo = null;

		#endregion [ Fields ]

		#region [ Constructors ]

		/// <summary>Constructor with some algorithm</summary>
		public SymmetricEncryptor( SymmetricAlgoType algo )
		{
			this.CreateEncryptor( algo );
		}

		/// <summary>Selectes the correct algorithm to instantiate</summary>
		/// <param name="type">The algorithm to use</param>
		private void CreateEncryptor( SymmetricAlgoType type )
		{
			// set up the encryptor
			switch (type)
            {
				case SymmetricAlgoType.DES:
					this._algo = new DESCryptoServiceProvider();
					break;
				case SymmetricAlgoType.TripleDES:
					this._algo = new TripleDESCryptoServiceProvider();
					break;
				case SymmetricAlgoType.RC2:
					this._algo = new RC2CryptoServiceProvider();
					break;
				case SymmetricAlgoType.AES:
				default:
					this._algo = new RijndaelManaged();
					break;
			}
		}

        #endregion [ Constructors ]

        #region [ Properties ]

        /// <summary>Get the legal key sizes of the instantiated algorithm.</summary>
        public KeySizes KeySizes
        {
            get { return this.Algorithm.LegalKeySizes[0]; }
        }

		/// <summary>The encryptor</summary>
		protected SymmetricAlgorithm Algorithm
        {
			get { return this._algo;  }
        }

		#endregion [ Properties ]

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

		/// <summary>Encrypt plain text with a known key but generate an IV</summary>
        /// <param name="plainText">text to encrypt</param>
        /// <param name="key">encryption key to use</param>
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

		/// <summary>Encrypt JUST plain bytes with some key</summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
		public CipherMessage Encrypt( byte[] data, byte[] key )
        {
			// validate arguments
			if ( data == null || data.Length < 1 )
				throw new System.ArgumentNullException( nameof( data ) );

			// need better validation of the key size
			if ( key == null || key.Length < 1 )
				throw new System.ArgumentNullException( nameof( key ) );

			// setup the algorithm
			this.Algorithm.Mode = CipherMode.CBC;
			//this.Algorithm.KeySize = 128;
			//this.Algorithm.BlockSize = 128;
			//this.Algorithm.FeedbackSize = 128;
			//this.Algorithm.Padding = PaddingMode.Zeros;
			this.Algorithm.GenerateIV();
            this.Algorithm.Key = key;

            CipherMessage results = new CipherMessage();

			// create an encryptor
			ICryptoTransform encryptor = this.Algorithm.CreateEncryptor( this.Algorithm.Key, this.Algorithm.IV );

			using ( var ms = new MemoryStream() )
			using ( var cs = new CryptoStream( ms, encryptor, CryptoStreamMode.Write ) )
			{
				cs.Write( data, 0, data.Length );
				cs.FlushFinalBlock();

				results.CipherBytes = ms.ToArray();
			}

			results.IV = this.Algorithm.IV;
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

		/// <summary>zero out memory</summary>
		public void Clear()
		{
			// Clear the RijndaelManaged object.
			if ( this.Algorithm != null )
				this.Algorithm.Clear();
		}
	}
}