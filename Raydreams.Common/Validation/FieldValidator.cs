using System;
using System.Text.RegularExpressions;

namespace Raydreams.Common.Validation
{
	/// <summary>Interface all the validators need for polymorphism.</summary>
	public interface IFieldValidator<T>
	{
		bool Test( T fieldValue );
	}

	/// <summary>test that a field has a value</summary>
	public class MissingFieldValidator : IFieldValidator<String>
	{
		/// <summary></summary>
		/// <param name="fieldValue"></param>
		/// <returns>Return true if the test passes and is valid.</returns>
		public bool Test( string fieldValue )
		{
			return (!String.IsNullOrWhiteSpace( fieldValue ));
		}
	}

	/// <summary>Test for a min/max length string</summary>
	public class MinMaxLength : IFieldValidator<String>
	{
		public int Min { get; set; }
		public int Max { get; set; }

		public MinMaxLength( int min, int max )
		{
			this.Min = min;
			this.Max = max;
		}

		public bool Test( string fieldValue )
		{
			if ( this.Min > 0 && fieldValue.Length < this.Min )
				return false;

			if ( this.Max > 0 && fieldValue.Length > this.Max )
				return false;

			return true;
		}
	}

	/// <summary>Has to be an exact length only</summary>
	public class ExactLength : MinMaxLength
	{
		public ExactLength( int len ) : base( len, len )
		{
		}
	}

	/// <summary>Validate with a regualr expression</summary>
	public class RegExValidator : IFieldValidator<String>
	{
		public Regex Pattern { get; set; }

		public RegExValidator( string pattern, bool ignoreCase = false )
		{
			RegexOptions ops = RegexOptions.None;

			if ( ignoreCase )
				ops = ops | RegexOptions.IgnoreCase;

			this.Pattern = new Regex( pattern, ops );
		}

		/// <summary>Test against a specific pattern. If the sent value is null, will not match unless the pattern is also null</summary>
		/// <param name="fieldValue"></param>
		/// <returns></returns>
		public bool Test( string fieldValue )
		{
			// only matches null if the pattern was not set and is null
			if ( fieldValue == null )
			{
				return (this.Pattern == null);
			}

			// otherwise match the pattern
			return this.Pattern.IsMatch( fieldValue );
		}
	}

}
