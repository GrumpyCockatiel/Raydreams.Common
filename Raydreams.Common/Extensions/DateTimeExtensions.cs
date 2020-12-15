using System;

namespace Raydreams.Common.Extensions
{
    /// <summary></summary>
    public static class DateTimeExtensions
    {
        /// <summary>Enumerates the quarters of a year.</summary>
        public enum DateQuarterValue
        {
            Unknown = 0,
            Q1 = 1,
            Q2 = 2,
            Q3 = 3,
            Q4 = 4
        }

        /// <summary>Sets the Kind to UTC</summary>
        /// <returns></returns>
        public static DateTime SetAsUtc(this DateTime input)
        {
            return new DateTime(input.Year, input.Month, input.Day, input.Hour, input.Minute, input.Second, input.Millisecond, DateTimeKind.Utc);
        }

        /// <summary>Returns the current quarter</summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateQuarterValue Quarter(this DateTime dateTime)
        {
            int quarter = (dateTime.Month - 1) / 3 + 1;
            return (DateQuarterValue)quarter;
        }

        /// <summary>Returns the date normalized to the last second of the year defaulting to UTC.</summary>
        /// <returns></returns>
        public static DateTime EndOfYear(this DateTime input, DateTimeKind kind = DateTimeKind.Utc)
        {
            return new DateTime(input.Year, 12, 31, 23, 59, 59, kind);
        }

        /// <summary>Returns the date normalized to the last second of the specified day default to UTC.</summary>
        /// <returns></returns>
        public static DateTime EndOfDay(this DateTime input, DateTimeKind kind = DateTimeKind.Utc)
        {
            return new DateTime(input.Year, input.Month, input.Day, 23, 59, 59, 999, kind);
        }

        /// <summary>Returns the date normalized to the last second of the specified day default to UTC.</summary>
        /// <returns></returns>
        public static DateTime StartOfDay(this DateTime input, DateTimeKind kind = DateTimeKind.Utc)
        {
            return new DateTime(input.Year, input.Month, input.Day, 0, 0, 0, 0, kind);
        }

        /// <summary>Normalizes date to the beginning of the day. Creates a new Date but truncates the hours, min and seconds.</summary>
        /// <returns></returns>
        public static DateTime TruncateHours(this DateTime input)
        {
            return new DateTime(input.Year, input.Month, input.Day);
        }

        /// <summary>Test two dates to see if they are on the same day.</summary>
        /// <returns>True if year and day match, else false</returns>
        /// <remarks>Does not check time zones or conver to UTC first.</remarks>
        public static bool IsSameDay(this DateTime a, DateTime b)
        {
            return (a.Year == b.Year && a.DayOfYear == b.DayOfYear);
        }

        /// <summary>Calculates the Julian Day from a DateTime object.</summary>
        /// <param name="d">DateTime object from which to calculate a Julian Day.</param>
        /// <returns>A fractional Julian Day value.</returns>
        /// <remarks>JDN 0 is NOON on Jan 1, 4713 BC whic his a Monday, this JDN 0.5 is midnight Jan 2, 4713</remarks>
        public static double Date2Julian(this DateTime d)
        {
            double a, b;
            int theMonth = d.Month;
            int theYear = d.Year;

            if (d.Month <= 2)
            { --theYear; theMonth += 12; }

            a = Math.Floor(theYear / 100.0);

            if (d.Year < 1582)
                b = 0;
            else if (d.Year > 1582)
                b = 2 - a + Math.Floor(a / 4);
            else
            {
                if (d.Month < 10)
                    b = 0;
                else if (d.Month > 10)
                    b = 2 - a + Math.Floor(a / 4);
                else
                {
                    if (d.Day < 5)
                        b = 0;
                    else if (d.Day >= 15)
                        b = 2 - a + Math.Floor(a / 4);
                    else
                        throw new System.Exception("Input day falls between 10/5/1582 and 10/14/1582 which does not exist in the Gregorian Calendar.");
                } // end middle else
            } // end outer else

            double jd = (Math.Floor(365.25 * (theYear + 4716)) + Math.Floor(30.6001 * (theMonth + 1)) + d.Day + b - 1524.5);

            // add fractional parts of the day to the Julian Day
            TimeSpan span = TimeSpan.FromHours(d.Hour) + TimeSpan.FromMinutes(d.Minute) + TimeSpan.FromSeconds(d.Second) + TimeSpan.FromMilliseconds(d.Millisecond);

            return jd + span.TotalDays;
        }
    }
}
