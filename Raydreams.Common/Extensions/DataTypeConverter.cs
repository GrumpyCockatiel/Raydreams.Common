using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Raydreams.Common.Extensions
{
    /// <summary>Extensions to more robustly convert data types from a string.</summary>
    public static class DataTypeConverter
	{
		/// <summary></summary>
		/// <returns></returns>
		/// <param name="def">Default value to return when a fail occurs</param>
		public static int GetIntValue( this string value, int def = 0 )
		{
			int convert = 0;

			if ( Int32.TryParse( value, out convert ) )
				return convert;

			return def;
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="def">Default value to return when a fail occurs</param>
		public static int? GetNullableIntValue( this string value )
		{
			int convert = 0;

			if ( Int32.TryParse( value, out convert ) )
				return convert;

			return null;
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="def">Default value to return when a fail occurs</param>
		public static long GetLongValue( this string value, long def = 0 )
		{
			long convert = 0;

			if ( Int64.TryParse( value, out convert ) )
				return convert;

			return def;
		}

		/// <summary>Converts a string to a boolean value based on the first char</summary>
		/// <returns></returns>
		public static bool GetBooleanValue( this string value )
		{
			if ( String.IsNullOrWhiteSpace( value ) )
				return false;

			char leading = value.Trim().ToLower()[0];

			return ( leading == 't' || leading == 'y' || leading == '1' ) ? true : false;
		}

        /// <summary>Converts a string to a boolean value based on the first char</summary>
        /// <returns></returns>
        public static bool? GetNullableBooleanValue(this string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return null;

            char leading = value.Trim().ToLower()[0];

            return (leading == 't' || leading == 'y' || leading == '1') ? true : false;
        }

        /// <summary></summary>
        /// <returns></returns>
        public static double GetDoubleValue(this string value)
        {
            double result = 0;

            if (String.IsNullOrWhiteSpace(value))
                return result;

            if (Double.TryParse(value, out result))
                return result;

            return 0;
        }

        /// <summary></summary>
        /// <returns></returns>
        public static double? GetNullableDoubleValue(this string value)
        {
            double convert = 0;

            if (Double.TryParse(value, out convert))
                return convert;

            return null;
        }

        /// <summary></summary>
        /// <returns></returns>
        public static float GetFloatValue(this string value)
        {
            float result = 0;

            if (String.IsNullOrWhiteSpace( value ))
                return result;

            if (Single.TryParse( value, out result ))
                return result;

            return 0;
        }

        /// <summary>Parses a string to DateTime, returning DateTime min if it can not be parsed.</summary>
        /// <param name="value">The input date time string</param>
        /// <returns></returns>
        /// <remarks>Can convert a ISO8601 2016-12-05T00:00:00Z formatted string</remarks>
        public static DateTime GetDateTimeValue(this string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return DateTime.MinValue;

            DateTime result = new DateTime();

			if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AllowWhiteSpaces, out result))
				return result;

            return DateTime.MinValue;
        }

        /// <summary>Parses a string to DateTime, returning null if it can not be parsed.</summary>
        /// <returns></returns>
        public static DateTime? GetNullDateTimeValue(this string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return null;

            DateTime result = new DateTime();

			if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AllowWhiteSpaces, out result))
				return result;

            return null;
        }

        /// <summary></summary>
        /// <returns></returns>
        public static T GetEnumValue<T>(this string value) where T : struct, IConvertible
        {
            T result = default(T);

            if (String.IsNullOrWhiteSpace(value))
                return result;

            if (Enum.TryParse<T>(value, true, out result))
                return result;

            return default(T);
        }

        /// <summary></summary>
		/// <returns></returns>
		public static ObjectId GetObjectIdValue( this string value )
		{
			if ( String.IsNullOrWhiteSpace( value ) )
				return ObjectId.Empty;

			value = value.Trim();

			ObjectId id = ObjectId.Empty;
			if ( ObjectId.TryParse( value, out id ) )
				return id;

			return ObjectId.Empty;
		}


    }
}