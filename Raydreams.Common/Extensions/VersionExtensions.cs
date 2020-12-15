using System;

namespace Raydreams.Common.Extensions
{
	/// <summary></summary>
	public static class VersionExtensions
	{
		/// <summary>Returns the build date as a string version.</summary>
		/// <param name="ver"></param>
		/// <returns></returns>
		/// <remarks>Assumes the build occured after 1/1/2000.</remarks>
		public static string ParseLastBuildDate(this Version ver)
		{
			DateTime buildDate = new DateTime(2000, 1, 1).AddDays(ver.Build).AddSeconds(ver.Revision * 2);
			return buildDate.ToString(); // ToLocalTime().ToString( CultureInfo.InvariantCulture );
		}
	}
}
