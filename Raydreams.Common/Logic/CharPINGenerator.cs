using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raydreams.Common.Collections;
using Raydreams.Common.Extensions;

namespace Raydreams.Common.Logic
{
	/// <summary>Interaface to create a new generator</summary>
	public interface INetIDGenerator
	{
		/// <summary>Base methods to call to generate a new ID</summary>
		/// <param name="fname">Required first name of at least 1 char</param>
		/// <param name="lname">Required last name of at least 1 char</param>
		/// <param name="mname">Optional middle name or letter to use in the algorithm if provided</param>
		/// <returns></returns>
		string Generate(string fname, string lname, string mname = null);
	}

	/// <summary>Generates a CharPIN ID based on a persons name(s). Tries to come up with something that has similar letters</summary>
	/// <remarks>John Doe might get JDXEY</remarks>
	public class CharPINGenerator : INetIDGenerator
	{
		private static List<String> _vulgarWords = null;

		private Randomizer _util = null;

		/// <summary>Example static constructor to preload the vulgar words from a file</summary>
		static CharPINGenerator()
		{
			//VulgarWords = ProfanityDictionary.All;
		}

		/// <summary>Generate a CharPIN of the specified length</summary>
		/// <param name="length">The fixed length the ID should be</param>
		/// <param name="filter">The list of words to not permit in the result. Send null to use the entire Profanity Dictionary.</param>
		public CharPINGenerator(int length)
		{
			this.PINLength = (length < 3 || length > 10) ? 5 : length;

		   this._util = new Randomizer();
		}

		#region [Properties]

		/// <summary>Protocol to use to test if the ID that has been created already exists</summary>
		//public IDAlreadyExists DoCheckExists { get; set; }

		/// <summary>Lenght of the PIN</summary>
		public int PINLength = 5;

		/// <summary>Vulgar Words filter list to use to void bad words or PIN words that cannot be used</summary>
		public static List<String> VulgarWords
		{
			set
			{
				_vulgarWords = value;
			}
			get
			{
				if (_vulgarWords == null)
					//return ProfanityDictionary.All;
					return new List<string>();

				return _vulgarWords;
			}
		}

		#endregion [Properties]

		#region [Methods]

		/// <summary>Is any substring in the bad word list</summary>
		public static bool IsVulgar(string id)
		{
			return VulgarWords.Any(s => id.ToLower().Contains(s.ToLower()));
		}

		/// <summary>Contains the logic for generating a new ID from the input names</summary>
		/// <param name="fname"></param>
		/// <param name="lname"></param>
		/// <returns></returns>
		public string Generate( string fname, string lname, string mname = null )
		{
			if (String.IsNullOrWhiteSpace(fname) && String.IsNullOrWhiteSpace(lname))
				throw new System.ArgumentException("Both first and last name can not be null or empty.");

			// seed the array and make it random
			char[] current = Enumerable.Repeat<char>(Char.MinValue, this.PINLength).ToArray<char>();

			List<string> names = new List<string>();

			if (!String.IsNullOrWhiteSpace(fname))
				names.Add( fname.RemoveNonChar() );

			if (!String.IsNullOrWhiteSpace(mname))
				names.Add( mname.RemoveNonChar() );

			if (!String.IsNullOrWhiteSpace(lname))
				names.Add( lname.RemoveNonChar() );

			if (names.Count < 2)
				this.OneName(current, names[0]);
			else if (names.Count < 3)
				this.TwoNames(current, names[0], names[1]);
			else
				this.AllNames(current, names[0], names[1], names[2]);

			return new String(current).ToUpper();
		}

		/// <summary>Logic if user only has one name</summary>
		private void OneName(char[] current, string name)
		{
			current[0] = name[0];
			List<char> post = new List<char>(this._util.PadMinLength(name.Substring(1), this.PINLength));
			ShuffledDeck<char> deck = new ShuffledDeck<char>(post, this._util.Generator);

			IEnumerator<char> shoe = deck.GetEnumerator();
			shoe.MoveNext();
			for (int i = 1; i < current.Length; ++i)
			{
				current[i] = shoe.Current;
				shoe.MoveNext();
			}
		}

		/// <summary>Logic if the user has two names</summary>
		private void TwoNames(char[] current, string fname, string lname)
		{
			// mark the indices
			int mid = this.GetMidpoint(fname.Length, lname.Length);

			current[0] = fname[0];
			current[mid] = lname[0];

			List<char> pre = new List<char>(this._util.PadMinLength(fname.Substring(1), this.PINLength));
			List<char> post = new List<char>(this._util.PadMinLength(lname.Substring(1), this.PINLength));

			ShuffledDeck<char> deck = new ShuffledDeck<char>(pre, this._util.Generator);

			IEnumerator<char> shoe = deck.GetEnumerator();
			shoe.MoveNext();
			for (int i = 1; i < mid; ++i)
			{
				current[i] = shoe.Current;
				shoe.MoveNext();
			}

			deck = new ShuffledDeck<char>(post, this._util.Generator);
			shoe = deck.GetEnumerator();
			shoe.MoveNext();
			for (int i = mid + 1; i < current.Length; ++i)
			{
				current[i] = shoe.Current;
				shoe.MoveNext();
			}
		}

		/// <summary>User has a first, middle and last name</summary>
        /// <remarks>Spaces are removed and collapsed so multiple middle names still work</remarks>
		private void AllNames(char[] current, string fname, string mname, string lname)
		{
			// mark the indices
			int mid = this.GetMidpoint(fname.Length, lname.Length);

			current[0] = fname[0];
			current[mid] = mname[0];
			current[mid + 1] = lname[0];

			List<char> pre = new List<char>(this._util.PadMinLength(fname.Substring(1), this.PINLength));
			List<char> post = new List<char>(this._util.PadMinLength(lname.Substring(1), this.PINLength));

			ShuffledDeck<char> deck = new ShuffledDeck<char>(pre, this._util.Generator);
			IEnumerator<char> shoe = deck.GetEnumerator();
			shoe.MoveNext();

			for (int i = 1; i < mid; ++i)
			{
				current[i] = shoe.Current;
				shoe.MoveNext();
			}

			deck = new ShuffledDeck<char>(post, this._util.Generator);
			shoe = deck.GetEnumerator();
			shoe.MoveNext();

			for (int i = mid + 2; i < current.Length; ++i)
			{
				current[i] = shoe.Current;
				shoe.MoveNext();
			}
		}

		/// <summary>Determines the midpoint char of a PIN</summary>
		/// <param name="pre"></param>
		/// <param name="post"></param>
		/// <returns></returns>
		private int GetMidpoint(int pre, int post)
		{
			if (pre < 1 || post < 1)
				return 0;

			if (this.PINLength % 2 == 0)
				return this.PINLength / 2;

			return (pre <= post) ? this.PINLength / 2 : (this.PINLength / 2) + 1;
		}

		#endregion [Methods]
	}
}
