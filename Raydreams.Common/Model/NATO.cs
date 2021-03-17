namespace Raydreams.Common.Model
{
	/// <summary>String lookup of the NATO call sign values.</summary>
    /// <remarks>This mainly to be plugged into a randomizer to make a random Folio name e.g. Tango431</remarks>
	public static class NATO
	{
		private static readonly string[] _nato = {"Alpha","Bravo","Charlie","Delta","Echo","Foxtrot","Golf","Hotel","India","Juliet",
				"Kilo","Lima","Mike","November","Oscar","Papa","Quebec","Romeo","Sierra","Tango","Uniform","Victor","Whiskey","X-ray",
				"Yankee","Zulu"};

		static NATO() { }

		/// <summary>Returns the NATO alphabet list</summary>
		public static string[] Values
		{
			get { return _nato; }
		}

	}
}
