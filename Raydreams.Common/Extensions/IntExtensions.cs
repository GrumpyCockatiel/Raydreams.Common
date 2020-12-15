using System;

namespace Raydreams.Common.Extensions
{
    public static class IntExtensions
    {
		/// <summary>Normalize Inclusive where default is min</summary>
		/// <returns></returns>
		public static int Normalize(this int x, int min, int max)
        {
			// swap check
			if (max < min)
            {
				int temp = min;
				min = max;
				max = temp;
            }

			if (x < min)
				return min;

			if (x > max)
				return min;

			return x;
		}

		/// <summary>Normalize Inclusive with a default on fail</summary>
		/// <returns></returns>
		public static int NormalizeDefault(this int x, int min, int max, int def = 0)
		{
			// swap check
			if (max < min)
			{
				int temp = min;
				min = max;
				max = temp;
			}

			if (x < min)
				return def;

			if (x > max)
				return def;

			return x;
		}

		/// <summary>TZ must be [-12, 12] otherwise assume its UTC</summary>
		public static int NormalizeTZ(this int tz)
		{
			if (tz < -12)
				return 0;

			if (tz > 12)
				return 0;

			return tz;
		}

		/// <summary>Normailizes a month integer to [1,12]</summary>
		public static int NormalizeMonth(this int month)
		{
			if (month < 1)
				return 1;

			if (month > 12)
				return 12;

			return month;
		}

		/// <summary>Makes sure the month integer is legit</summary>
		public static int NormalizeYear(this int year)
		{
			if (year < 1900)
				return DateTime.UtcNow.Year;

			if (year > DateTime.UtcNow.Year)
				return DateTime.UtcNow.Year;

			return year;
		}
	}
}
