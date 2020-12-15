using System;
using System.Text;

namespace Raydreams.Common.Extensions
{
	/// <summary></summary>
	public static class ArrayExtensions
	{
		// returns a byte array as a hex string
		public static string ToHexString(this byte[] hex)
		{
			if (hex == null)
				return null;

			if (hex.Length == 0)
				return string.Empty;

			StringBuilder s = new StringBuilder();

			foreach (byte b in hex)
				s.Append(b.ToString("x2"));

			return s.ToString();
		}

		/// <summary>Returns a string array with the specified quote char and separator char</summary>
		/// <param name="ary">The string array to join.</param>
		/// <param name="separator">The separator character to use between each</param>
		/// <param name="quote">The quote character to use, doesnt have to be a literal quotation</param>
		/// <returns></returns>
		public static string JoinWithQuotes( this string[] ary, char separator, char quote )
		{
			StringBuilder sb = new StringBuilder();

			foreach ( string s in ary )
			{
				if ( String.IsNullOrWhiteSpace( s ) )
					continue;

				sb.AppendFormat( "{0}{1}{0}{2}", quote, s.Trim(), separator );
			}

			// remove the last separator
			--sb.Length;

			return sb.ToString();
		}

		/// <summary>Returns a string array with the specified quote char and separator char</summary>
		/// <param name="ary">The string array to join.</param>
		/// <param name="separator">The separator character to use between each</param>
		/// <param name="quote">The quote character to use, doesnt have to be a literal quotation</param>
		/// <returns></returns>
		public static string JoinWithQuotes( this Guid[] ary, char separator, char quote )
		{
			StringBuilder sb = new StringBuilder();

			foreach ( Guid s in ary )
			{
				sb.AppendFormat( "{0}{1}{0}{2}", quote, s, separator );
			}

			// remove the last separator
			--sb.Length;

			return sb.ToString();
		}
	}
}
