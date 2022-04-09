using System;

namespace Raydreams.Common.Extensions
{
	/// <summary>Returns a value at midnight of the given month and year</summary>
	public static class DateTimeOffsetExtensions
	{
		public static DateTimeOffset FirstOfMonth( int year, int month )
		{
			if ( month < 1 )
				month = 1;

			if ( month > 12 )
				month = 12;

			return new DateTimeOffset( year, month, 1, 0, 0, 0, new TimeSpan() );
		}

		/// <summary>Sets the last millisecond in the month at UTC but still in that month</summary>
		/// <param name="year">Year.</param>
		/// <param name="month">Month.</param>
		public static DateTimeOffset EndOfMonth( int year, int month )
		{
			return new DateTimeOffset( year, month, DateTime.DaysInMonth( year, month ), 23, 59, 59, 999, new TimeSpan() );
		}

		/// <summary>Test two dates to see if they are on the same day.</summary>
		/// <returns>True if year and day match, else false</returns>
		/// <remarks>Does not check time zones or convert to UTC first.</remarks>
		public static bool IsSameMonth( this DateTimeOffset a, DateTimeOffset b )
		{
			return ( a.Year == b.Year && a.Month == b.Month );
		}

		/// <summary>Modifies a value to midnight of the given month and year</summary>
		public static DateTimeOffset FirstOfMonth( this DateTimeOffset a )
		{
			return new DateTimeOffset( a.Year, a.Month, 1, 0, 0, 0, new TimeSpan() );
		}

		/// <summary>Returns the very beginning of the next month from the specified month</summary>
		/// <returns></returns>
		/// <remarks>TZ is set to UTC</remarks>
		public static DateTimeOffset FistOfNextMonth( int year, int month )
		{
			return FirstOfMonth( year, month ).FistOfNextMonth();
		}

		/// <summary>Returns the very beginning of the next month from the specified month</summary>
		/// <returns></returns>
		/// <remarks>TZ is set to UTC</remarks>
		public static DateTimeOffset FistOfNextMonth( this DateTimeOffset a )
		{
			DateTimeOffset next = a.AddMonths( 1 );

			return new DateTimeOffset( next.Year, next.Month, 1, 0, 0, 0, new TimeSpan() );
		}

		/// <summary>Returns the date normalized to the last second of the specified day default to UTC.</summary>
		/// <returns></returns>
		public static DateTimeOffset EndOfDay( this DateTimeOffset input )
		{
			return new DateTimeOffset( input.Year, input.Month, input.Day, 23, 59, 59, 999, input.Offset );
		}

		/// <summary>Returns the date normalized to the last second of the specified day default to UTC.</summary>
		/// <returns></returns>
		public static DateTimeOffset StartOfDay( this DateTimeOffset input )
		{
			return new DateTimeOffset( input.Year, input.Month, input.Day, 0, 0, 0, 0, input.Offset );
		}
	}

}