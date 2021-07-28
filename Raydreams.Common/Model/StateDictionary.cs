using System;
using System.Collections.Generic;
using System.Linq;

namespace Raydreams.Common.Utilities
{
	/// <summary> dictionary of all US State codes.</summary>
	public static class StateDictionary
	{
		/// <summary>US State Codes</summary>
		public static Dictionary<string, string> States = null;

		/// <summary>Canadian Territory Codes</summary>
		public static Dictionary<string, string> Territories = null;

        /// <summary>All Postal Codes</summary>
		public static Dictionary<string, string> All = null;

        static StateDictionary()
		{
            States = new Dictionary<string, string>
            {
                { "AL", "Alabama" },
                { "AK", "Alaska" },
                { "AZ", "Arizona" },
                { "AR", "Arkansas" },
                { "CA", "California" },
                { "CO", "Colorado" },
                { "CT", "Connecticut" },
                { "DE", "Delaware" },
                { "DC", "District Of Columbia" },
                { "FL", "Florida" },
                { "GA", "Georgia" },
                { "HI", "Hawaii" },
                { "ID", "Idaho" },
                { "IL", "Illinois" },
                { "IN", "Indiana" },
                { "IA", "Iowa" },
                { "KS", "Kansas" },
                { "KY", "Kentucky" },
                { "LA", "Louisiana" },
                { "ME", "Maine" },
                { "MD", "Maryland" },
                { "MA", "Massachusetts" },
                { "MI", "Michigan" },
                { "MN", "Minnesota" },
                { "MS", "Mississippi" },
                { "MO", "Missouri" },
                { "MT", "Montana" },
                { "NE", "Nebraska" },
                { "NV", "Nevada" },
                { "NH", "New Hampshire" },
                { "NJ", "New Jersey" },
                { "NM", "New Mexico" },
                { "NY", "New York" },
                { "NC", "North Carolina" },
                { "ND", "North Dakota" },
                { "OH", "Ohio" },
                { "OK", "Oklahoma" },
                { "OR", "Oregon" },
                { "PA", "Pennsylvania" },
                { "RI", "Rhode Island" },
                { "SC", "South Carolina" },
                { "SD", "South Dakota" },
                { "TN", "Tennessee" },
                { "TX", "Texas" },
                { "UT", "Utah" },
                { "VT", "Vermont" },
                { "VA", "Virginia" },
                { "WA", "Washington" },
                { "WV", "West Virginia" },
                { "WI", "Wisconsin" },
                { "WY", "Wyoming" }
            };

            Territories = new Dictionary<string, string>()
			{
				{"AB", "Alberta"},
				{"BC", "British Columbia"},
				{"MB", "Manitoba"},
				{"NB", "New Brunswick"},
				{"NL", "Newfoundland and Labrador"},
				{"NT", "Northwest Territories"},
				{"NS", "Nova Scotia"},
				{"NU", "Nunavut"},
				{"ON", "Ontario"},
				{"PE", "Prince Edward Island"},
				{"QC", "Quebec"},
				{"SK", "Saskatchewan"},
				{"YT", "Yukon"}
			};

            All = States.Union( Territories ).ToDictionary( s => s.Key, s => s.Value );
		}

		/// <summary>Gets the two letter State Code based on the full state name.</summary>
		/// <param name="state">The full state name, case ignored.</param>    
		/// <returns>The 2 letter state code.</returns>
		public static string GetCode( string state )
		{
			if ( String.IsNullOrWhiteSpace( state ) )
				return null;

			KeyValuePair<string, string> entry = All.Where( v => v.Value.Equals( state, StringComparison.InvariantCultureIgnoreCase ) ).FirstOrDefault();

			return entry.Key ?? String.Empty;
		}

		/// <summary>Test for a valid two letter state code where case does not matter.</summary>
		/// <returns><c>true</c>, if valid code was valid, <c>false</c> otherwise.</returns>
		/// <param name="code">Two letter state code.</param>
		public static bool IsValidCode( string code )
		{
			if ( String.IsNullOrWhiteSpace( code ) )
				return false;

			code = code.Trim().ToUpper();

			return States.ContainsKey( code ) || Territories.ContainsKey(code);
		}
	}
}
