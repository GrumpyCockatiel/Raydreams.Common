using System;

namespace Raydreams.Common.Extensions
{
	/// <summary>Extensions to the Version class</summary>
	public static class VersionExtensions
	{
		/// <summary>Parses a decimal build date to a date time string</summary>
		/// <param name="ver">The version object</param>
		/// <returns>DateTime as string</returns>
		/// <remarks>Assumes the build occured after 1/1/2000 which is the base date.</remarks>
		public static string ParseLastBuildDate(this Version ver)
		{
			return new DateTime(2000, 1, 1).AddDays(ver.Build).AddSeconds(ver.Revision * 2).ToString();
			// ToLocalTime().ToString( CultureInfo.InvariantCulture );
		}
	}
}
