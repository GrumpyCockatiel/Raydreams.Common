using System;
using System.Collections.Generic;
using System.Linq;

namespace Raydreams.Common.Utilities
{
	/// <summary> dictionary of all US State codes.</summary>
	public static class StateDictionary
	{
		public static Dictionary<string, string> States = null;
	
		static StateDictionary()
		{
			States = new Dictionary<string, string>();

			States.Add( "AL", "Alabama" );
			States.Add( "AK", "Alaska" );
			States.Add( "AZ", "Arizona" );
			States.Add( "AR", "Arkansas" );
			States.Add( "CA", "California" );
			States.Add( "CO", "Colorado" );
			States.Add( "CT", "Connecticut" );
			States.Add( "DE", "Delaware" );
			States.Add( "DC", "District Of Columbia" );
			States.Add( "FL", "Florida" );
			States.Add( "GA", "Georgia" );
			States.Add( "HI", "Hawaii" );
			States.Add( "ID", "Idaho" );
			States.Add( "IL", "Illinois" );
			States.Add( "IN", "Indiana" );
			States.Add( "IA", "Iowa" );
			States.Add( "KS", "Kansas" );
			States.Add( "KY", "Kentucky" );
			States.Add( "LA", "Louisiana" );
			States.Add( "ME", "Maine" );
			States.Add( "MD", "Maryland" );
			States.Add( "MA", "Massachusetts" );
			States.Add( "MI", "Michigan" );
			States.Add( "MN", "Minnesota" );
			States.Add( "MS", "Mississippi" );
			States.Add( "MO", "Missouri" );
			States.Add( "MT", "Montana" );
			States.Add( "NE", "Nebraska" );
			States.Add( "NV", "Nevada" );
			States.Add( "NH", "New Hampshire" );
			States.Add( "NJ", "New Jersey" );
			States.Add( "NM", "New Mexico" );
			States.Add( "NY", "New York" );
			States.Add( "NC", "North Carolina" );
			States.Add( "ND", "North Dakota" );
			States.Add( "OH", "Ohio" );
			States.Add( "OK", "Oklahoma" );
			States.Add( "OR", "Oregon" );
			States.Add( "PA", "Pennsylvania" );
			States.Add( "RI", "Rhode Island" );
			States.Add( "SC", "South Carolina" );
			States.Add( "SD", "South Dakota" );
			States.Add( "TN", "Tennessee" );
			States.Add( "TX", "Texas" );
			States.Add( "UT", "Utah" );
			States.Add( "VT", "Vermont" );
			States.Add( "VA", "Virginia" );
			States.Add( "WA", "Washington" );
			States.Add( "WV", "West Virginia" );
			States.Add( "WI", "Wisconsin" );
			States.Add( "WY", "Wyoming" );
		}

		/// <summary>Gets the two letter State Code based on the full state name.</summary>
		/// <param name="state">The full state name, case ignored.</param>    
		/// <returns>The 2 letter state code.</returns>
		public static string GetCode( string state )
		{
			if ( String.IsNullOrWhiteSpace( state ) )
				return null;

			KeyValuePair<string, string> entry = States.Where( v => v.Value.Equals( state, StringComparison.InvariantCultureIgnoreCase ) ).FirstOrDefault();

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

			return States.ContainsKey( code );
		}
	}
}
