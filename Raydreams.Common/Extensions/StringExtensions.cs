using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Raydreams.Common.Extensions
{
    /// <summary>Tons of string utility functions</summary>
    public static class StringExtensions
	{
        /// <summary>Gets JUST the filename part of a URL</summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetFilenameFromUrl( this string url )
        {
            return ( !url.Contains( "." ) ) ? String.Empty : Path.GetFileName( new Uri( url ).AbsolutePath );
        }

        /// <summary>Truncates a string to the the specified length or less</summary>
        public static string Truncate(this string str, int length, bool trim = true)
        {
            // if greater than length
            if (str.Length > length)
                return (trim) ? str.Trim().Substring(0, length) : str.Substring(0, length);

            return (trim) ? str.Trim() : str;
        }

        /// <summary>Formats a string as a US Phone number</summary>
        public static string FormatUSPhoneNo( this string str )
		{
			str = str.RemoveNonDigits();

			if ( str == String.Empty || str.Length < 10 )
				throw new System.ArgumentException( "String can not be formatted as a US phone number." );

			if ( str.Length == 10 )
				return String.Format( "({0}) {1}-{2}", str.Substring( 0, 3 ), str.Substring( 3, 3 ), str.Substring( 6, 4 ) );
			else if ( str.Length == 11 && str[0] == '1' )
				return String.Format( "({0}) {1}-{2}", str.Substring( 1, 3 ), str.Substring( 4, 3 ), str.Substring( 7, 4 ) );

			throw new System.ArgumentException( "String can not be formatted as a US phone number." );
		}

        /// <summary>Tests a list of strings to see if it contains any string from the sublist.</summary>
        /// <param name="list"></param>
        /// <param name="sublist"></param>
        /// <returns></returns>
        /// <remarks>return VulgarWords.Any( s => id.ToLower().Contains( s.ToLower() ) );</remarks>
        public static bool SublistContains(this List<string> list, string[] sublist)
        {
            IEnumerable<string> union = list.Intersect(sublist);
            return (union != null && union.Count() > 0);
        }

        /// <summary>If any string is null or white space, return true.</summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static bool IsAnyNullOrWhiteSpace( this string[] strs )
        {
            return strs.Count( s => String.IsNullOrWhiteSpace( s ) ) > 0;
        }

        /// <summary>If any string is null or white space, return false. A validation extension to test all strings in a params array</summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        [Obsolete( "Use IsAnyNullOrWhiteSpace instead" )]
        public static bool IsAllNotNullOrWhiteSpace(this string[] strs)
        {
            foreach (string s in strs)
                if (String.IsNullOrWhiteSpace(s))
                    return false;

            return true;
        }

        /// <summary>String formatting extension shortcut</summary>
        /// <param name="str"></param>
        /// <param name="values"></param>
        /// <returns>formatted string</returns>
        /// <reamarks>
        /// string s = "ID {0} has duplicate records".Formatter( id );
        /// made obsolete with the new C# $"{myVar} construct"
        /// </reamarks>
        [Obsolete("Use the new $ formatter.")]
        public static string Formatter(this string str, params object[] values)
		{
			return String.Format(str, values);
		}

        /// <summary>Test a string is a valid sAMAccount ID which can not be more than 25 chars for SAP</summary>
        public static bool IsValidSAMID(this System.String str)
        {
            Regex pattern = new Regex(@"^[.a-zA-Z0-9_-]*$", RegexOptions.IgnoreCase);
            return pattern.IsMatch(str);
        }

        /// <summary>Is a valid Object SID pattern</summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsValidSID( this System.String str )
		{
			if ( str.Trim() == String.Empty )
				return false;

			Regex pattern = new Regex( @"^S-\d-\d+-(\d+-){1,14}\d+$", RegexOptions.IgnoreCase );
			return pattern.IsMatch( str );
		}

        /// <summary>Is a valid Object SID pattern</summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsValidMongoID(this string str)
        {
            if (str.Trim() == String.Empty)
                return false;

            return new Regex(@"^[0-9a-fA-F]{24}$", RegexOptions.IgnoreCase).IsMatch(str);
        }

        /// <summary>Overlaps the specified end string with the string itself starting from the end. Not an append since the str length remains the same.</summary>
        /// <param name="str"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string ReplaceEndWith( this string str, string suffix )
		{
			if ( String.IsNullOrEmpty( suffix ) )
				return str;

			return String.Format( "{0}{1}", str.Substring( 0, str.Length - suffix.Length ), suffix );
		}

		/// <summary>Returns all of a string after the specified LAST occurance of the specified token</summary>
		/// <returns>The substring</returns>
		public static string GetLastAfter( this string str, char token )
		{
			if ( str.Length < 1 )
				return String.Empty;

			int idx = str.LastIndexOf( token );

			if ( idx < 0 || idx >= str.Length - 1 )
				return String.Empty;

			return str.Substring( idx + 1, str.Length - idx - 1 ).Trim();
		}

		/// <summary>removes the leading 'domain\' from a network ID</summary>
		public static string TrimDomainName( this string s )
		{
			string[] parts = s.Split( new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries );

			if ( parts.Length < 1 )
				return null;
			else if ( parts.Length == 1 )
				return parts[0].Trim();

			return parts[1].Trim();
		}

		/// <summary>Test and add a backslash to any string without one.</summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string TrailingBackslash(this System.String str)
        {
            str = str.Trim();
            return (str[str.Length - 1] == '\\') ? str : String.Format("{0}\\", str);
        }

        /// <summary>Test a string is nothing but letters and digits</summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsAlphanumeric(this System.String str)
        {
            return str.All(char.IsLetterOrDigit);
        }

        /// <summary>Test a string is an email address and then swaps the domain for the new one</summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceEmailDomain(this System.String str, string newDomain)
        {
            if (Regex.IsMatch(str, @"[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}", RegexOptions.IgnoreCase))
            {
                int at = str.IndexOf('@');
                string name = str.Substring(0, at);
                return String.Format("{0}{1}", name, newDomain);
            }

            return null;
        }

        /// <summary>returns the part of the email before the @</summary>
        /// <param name="str">The full email address in the format tag@gmail.com</param>
        /// <returns>The user part name of the email such as 'tag'</returns>
        public static string GetMailboxName(this System.String str)
        {
            try
            {
                return new MailAddress(str).User;
            }
            catch
            {
                // not ideal to handle in the catch block but...
                return null;
            }
        }

        /// <summary>Checks to see if the provided <see cref="fileName"/> contains a wild card (*)</summary>
        /// <param name="fileName"></param>
        /// <returns>True if a wild card exists, otherwise false</returns>
        public static bool IsWildCard( this string fileName )
		{
			return fileName.IndexOf( '*' ) >= 0;
		}

		/// <summary>Tries to cast a string to an Enum which could fail</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static T ToEnum<T>(this string value, bool ignoreCase = true)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        /// <summary>Converts a string to an enum value of enum T failing to default(T)</summary>
        /// <returns></returns>
        /// <remarks>Case is ignored</remarks>
        public static T GetEnumValue<T>( this string value ) where T : struct, IConvertible
        {
            T result = default( T );

            if ( String.IsNullOrWhiteSpace( value ) )
                return result;

            if ( Enum.TryParse<T>( value, true, out result ) )
                return result;

            return default( T );
        }

        /// <summary>Converts a string to an enum value with the specified default on fail</summary>
        /// <param name="def">Default value if parsing fails</param>
        /// <param name="ignoreCase">Ignore case by default</param>
        /// <returns></returns>
        public static T GetEnumValue<T>( this string value, T def, bool ignoreCase = true ) where T : struct, IConvertible
        {
            T result = def;

            if ( String.IsNullOrWhiteSpace( value ) )
                return result;

            if ( Enum.TryParse<T>( value, ignoreCase, out result ) )
                return result;

            return def;
        }

        /// <summary>Returns the enum value by the description string on the enum member</summary>
        public static T EnumByDescription<T>(this string desc) where T : struct
        {
            Type type = typeof(T);

            // is it an enum
            if (!type.IsEnum)
                throw new System.ArgumentException("Type must be an enum.");

            foreach (string field in Enum.GetNames(type))
            {
                MemberInfo[] infos = type.GetMember(field);

                foreach (MemberInfo info in infos)
                {
                    DescriptionAttribute attr = info.GetCustomAttribute<DescriptionAttribute>(false);
                    if (attr.Description.Equals(desc, StringComparison.InvariantCultureIgnoreCase))
                        return field.ToEnum<T>(true);
                }
            }

            return default(T);
        }

        /// <summary>Removes consecutive white space internally within a string.</summary>
        /// <param name="trim">Whether to trim whitespace from the front and back of the string as well.</param>
        public static string TrimConsecutive(this string str, bool trim = true)
        {
            if (trim)
                str = str.Trim();

            StringBuilder temp = new StringBuilder();

            for (int i = 0; i < str.Length; ++i)
            {
                if (i > 0 && Char.IsWhiteSpace(str[i]) && Char.IsWhiteSpace(temp[temp.Length - 1]))
                    continue;

                temp.Append(str[i]);
            }

            return temp.ToString();
        }

        /// <summary>Name cases a string so only the first letter of a word is upper and the rest are lower</summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string NameCase( this string str )
		{
			StringBuilder sb = new StringBuilder();
			bool upper = true;

			str = str.TrimConsecutive( true );

			// iterate the string
			for ( int i = 0; i < str.Length; ++i )
			{
				if ( upper )
					sb.Append( Char.ToUpper( str[i] ) );
				else
					sb.Append( Char.ToLower( str[i] ) );

				// if this char is whitespace then the next char will be upper
				upper = Char.IsWhiteSpace( str, i );
			}

			return sb.ToString();
		}

        /// <summary>Removes all whitespace characters from the string.</summary>
        public static string RemoveSpaces(this string str)
        {
            StringBuilder temp = new StringBuilder();

            foreach (char c in str)
            {
                if (!Char.IsWhiteSpace(c))
                    temp.Append(c);
            }

            return temp.ToString();
        }

        /// <summary>Given a string in the format key1=value1,key2=value2,key3=value3 splits into a dictionary</summary>
        public static Dictionary<string,string> PairsToDictionary( this string str, bool stripQuotes = true )
        {
            Dictionary<string, string> results = new Dictionary<string, string>();

            StringBuilder temp = new StringBuilder();

            foreach ( char c in str )
            {
                if ( c == ',' )
                {
                    string[] parts = temp.ToString().Split( '=', StringSplitOptions.None );
                    if ( parts != null && parts.Length > 0 && !String.IsNullOrWhiteSpace(parts[0]) )
                    {
                        parts[1] = ( parts.Length < 2 || String.IsNullOrWhiteSpace( parts[1] ) ) ? String.Empty : parts[1].Trim();
                        if ( stripQuotes )
                            parts[1] = parts[1].Replace( "\"", "" );
                        results.Add( parts[0].Trim(), parts[1] );
                    }
                        
                    temp = new StringBuilder();
                }
                else
                {
                    temp.Append( c );
                }
            }

            return results;
        }

        /// <summary>Cleans a string of all non digit characters and collapses to a string with no spaces.</summary>
        public static string RemoveNonDigits( this string str )
		{
			StringBuilder temp = new StringBuilder();

			foreach ( char c in str )
			{
				if ( Char.IsDigit( c ) )
					temp.Append( c );
			}

			return temp.ToString();
		}

        /// <summary>Test a char is a valid hexidecimal digit</summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsHex( this char c )
        {
            //return ( c >= '0' && c <= '9' ) || ( c >= 'a' && c <= 'f' ) || ( c >= 'A' && c <= 'F' );
            return System.Uri.IsHexDigit( c );
        }

        /// <summary>Cleans a string so that it only contains letters. No digits or spaces or special chars</summary>
        public static string RemoveNonChar(this string str)
        {
            return new String(str.Where(Char.IsLetter).ToArray());
        }

        /// <summary>Picks digits out of the specified string and return them as a list of numbers.</summary>
        /// <remarks>This can be done with LINQ now.</remarks>
        public static List<Int32> ParseDigits( this string str )
		{
			List<Int32> digits = new List<Int32>();
			StringBuilder temp = new StringBuilder();

			for ( int i = 0; i < str.Length; ++i )
			{
				while ( i < str.Length && Char.IsDigit( str[i] ) )
				{
					temp.Append( str[i++] );
				}

				if ( temp.Length > 0 )
				{
					digits.Add( Int32.Parse( temp.ToString() ) );
					temp.Length = 0;
				}
			}

			return digits;
		}

        /// <summary>Convert a string to a byte array of the specified length using ASCII</summary>
        /// <remarks>This only works with single byte encodings. Dont try with UTF-8</remarks>
        public static byte[] ToBytes(this string str, int len = 20)
        {
            byte[] ary = new byte[len];

            for (int i = 0; i < ary.Length; ++i)
                ary[i] = 0x0;

            Array.Copy(Encoding.ASCII.GetBytes(str), ary, System.Math.Min(20, str.Length));

            return ary;
        }

        /// <summary>Convenience function. Takes an array of strings as input.</summary>
        /// <param name="ignoreCase">If true, all the strings in the list will be lowered before calculation. False (default) preserves the case.</param>
        /// <param name="values">List of string to hash.</param>
        /// <returns>The MD5 digest converted to BASE64</returns>
        public static string HashAllToMD5(bool ignoreCase = false, params string[] values)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string s in values)
            {
                if (String.IsNullOrWhiteSpace(s))
                    continue;

                string t = s.Trim();

                if (ignoreCase)
                    t = t.ToLower();

                sb.Append(t);
            }

            byte[] hash = sb.ToString().HashToMD5();

            return Convert.ToBase64String(hash, Base64FormattingOptions.None);
        }

        /// <summary>Hash the string to an MD5 digest</summary>
        /// <param name="str">single input string to hash</param>
        /// <returns>byte array</returns>
        public static byte[] HashToMD5( this string str )
		{
			// calculate the hash value of the object
			byte[] hash;

			using ( MD5 hasher = MD5.Create() )
				hash = hasher.ComputeHash( Encoding.UTF8.GetBytes( str ) );

			return hash;
		}

        /// <summary>Hash the string to a SHA1 digest</summary>
        /// <param name="str">single input string to hash</param>
        /// <returns>byte array</returns>
        public static byte[] HashToSHA1(this string str)
        {
            // calculate the hash value of the object
            byte[] hash;

            using ( SHA1 haser = SHA1.Create() )
                hash = haser.ComputeHash( Encoding.UTF8.GetBytes( str ) );

            return hash;
        }

        /// <summary>Hash the string to a SHA256 digest</summary>
        /// <param name="str">single input string to hash</param>
        /// <returns>byte array</returns>
        public static byte[] HashToSHA256( this string str )
        {
            // calculate the hash value of the object
            byte[] hash;

            using ( SHA256 haser = SHA256.Create() )
                hash = haser.ComputeHash( Encoding.UTF8.GetBytes( str ) );

            return hash;
        }

        /// <summary>Encodes a byte array as BASE64 URL encoded</summary>
        public static string BASE64UrlEncode( byte[] arg )
        {
            string s = Convert.ToBase64String( arg ); // Regular base64 encoder
            //s = s.Split( '=' )[0];
            s = s.TrimEnd( '=' ); // remove any trailing =
            s = s.Replace( '+', '-' ); // 62nd char of encoding
            s = s.Replace( '/', '_' ); // 63rd char of encoding
            return s;
        }

        /// <summary>Decodes a BASE64 URL encoded string back to its original bytes</summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static byte[] BASE64UrlDecode( this string str )
        {
            str = str.Replace( '-', '+' ); // 62nd char of encoding
            str = str.Replace( '_', '/' ); // 63rd char of encoding

            // the modulus of the length by 4 can not be remaninder 1
            switch ( str.Length % 4 )
            {
                // no padding necessary
                case 0: break;
                // pad with two =
                case 2: str += "=="; break;
                // pad once
                case 3: str += "="; break;
                // hopefully this does not happen
                default:
                    throw new System.Exception( "Illegal BASE64URL string!" );
            }

            return Convert.FromBase64String( str ); // Standard base64 decoder
        }

        /// <summary>Custom string hasher that should the same results in every run</summary>
        /// <param name="str">Some string to hash</param>
        /// <returns>hash value as int</returns>
        /// <remarks>
        /// A bit of an example writing a good hash function
        /// https://andrewlock.net/why-is-string-gethashcode-different-each-time-i-run-my-program-in-net-core/
        /// </remarks>
        public static int GetDeterministicHashCode(this string str)
        {
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];

                    if (i == str.Length - 1)
                        break;

                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }
    }
}
