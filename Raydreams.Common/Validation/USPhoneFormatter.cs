using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Raydreams.Common.Validation
{
    /// <summary>Validates and Formats string as US Phone Numbers</summary>
    public static class USPhoneFormatter
    {
        /// <summary></summary>
        private static Dictionary<USPhoneFormat, (string, Regex)> _formats = null;

        /// <summary>Supported formats</summary>
        public enum USPhoneFormat
        {
            Standard = 0, // 123-456-7890
            AreaCode = 1, // (123) 456-7890
            Compact = 2, // 1234567890
            International = 3 // 1-123-456-7890
        }

        /// <summary>Constructor</summary>
        static USPhoneFormatter()
        {
            _formats = new Dictionary<USPhoneFormat, (string, Regex)>
            {
                { USPhoneFormat.Standard, ("{0}-{1}-{2}", new Regex( @"^\d{3}-\d{3}-\d{4}$", RegexOptions.IgnoreCase ) ) },
                { USPhoneFormat.AreaCode, ("({0}) {1}-{2}", new Regex( @"^\(\d\d\d\) \d{3}-\d{4}$", RegexOptions.IgnoreCase ) ) },
                { USPhoneFormat.Compact, ("{0}{1}{2}", new Regex( @"^\d{10}$", RegexOptions.IgnoreCase ) ) },
                { USPhoneFormat.International, ("1-{0}-{1}-{2}", new Regex( @"^1-\d{3}-\d{3}-\d{4}$", RegexOptions.IgnoreCase )) }
            };
        }

        /// <summary>Checks to see if the string can be formatted correctly.</summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsValid(string str)
        {
            if (String.IsNullOrWhiteSpace( str ))
                return false;

            // clean the string
            str = str.Cleanse();

            if (str.Length == 10)
                return true;
            else if (str.Length == 11 && str[0] == '1')
                return true;
            else
                return false;
        }

        /// <summary>Formats a string as a US Phone number</summary>
        /// <param name="format">Format to use, style 0 is default</param>
        /// <returns>Formatted string or all 000s if the input can not be formatted</returns>
        public static string Format(string str, USPhoneFormat format = USPhoneFormat.Standard)
        {
            string formatStr = _formats[format].Item1;

            if ( String.IsNullOrWhiteSpace(str) )
                return String.Format( formatStr, "000", "000", "0000" );

            // if already formatted then just bail
            if (_formats[format].Item2.IsMatch( str ))
                return str;

            // clean the string
            str = str.Cleanse();

            if (str == String.Empty || str.Length < 10)
                return String.Format( formatStr, "000", "000", "0000" );

            if (str.Length == 10)
                return String.Format( formatStr, str.Substring( 0, 3 ), str.Substring( 3, 3 ), str.Substring( 6, 4 ) );
            else if (str.Length == 11 && str[0] == '1')
                return String.Format( formatStr, str.Substring( 1, 3 ), str.Substring( 4, 3 ), str.Substring( 7, 4 ) );
            else
                return String.Format( formatStr, "000", "000", "0000" );
        }

        /// <summary>Cleans a string of all non digit characters and collapses to a string with no spaces.</summary>
        private static string Cleanse(this string str)
        {
            StringBuilder temp = new StringBuilder();

            foreach (char c in str)
                if (Char.IsDigit( c ))
                    temp.Append( c );

            return temp.ToString();
        }

    }
}
