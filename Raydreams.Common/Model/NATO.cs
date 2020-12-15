using System;
namespace Raydreams.Common.Model
{
	public static class NATO
	{
		private static readonly string[] _nato = {"Alpha","Bravo","Charlie","Delta","Echo","Foxtrot","Golf","Hotel","India","Juliet",
				"Kilo","Lima","Mike","November","Oscar","Papa","Quebec","Romeo","Sierra","Tango","Uniform","Victor","Whiskey","X-ray",
				"Yankee","Zulu"};

		static NATO()
		{
		}

		/// <summary>Returns the NATO alphabet list</summary>
		public static string[] Values
		{
			get { return _nato; }
		}

	}
}
