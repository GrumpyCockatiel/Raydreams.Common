using System;
using System.Text;

namespace Raydreams.Common.Logic
{
	/// <summary></summary>
    [Obsolete("Use Randomizer", true)]
	public class IDUtilities
	{
		private Random _rand = null;

		private static readonly string LowerCase = "abcdefghijklmnopqrstuvwxyz";
		private static readonly string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		/// <summary></summary>
		/// <param name="generator"></param>
		public IDUtilities(Random generator)
		{
			this._rand = generator ?? new Random(Guid.NewGuid().GetHashCode());
		}

		/// <summary></summary>
		public Random Generator
		{
			get { return this._rand;  }
		}

		/// <summary>Pick a single ramdom character</summary>
		/// <param name="upper">Returns lower case unless explicitly set to true</param>
		/// <returns></returns>
		public char RandomChar(bool upper = false)
		{
			if (upper)
				return UpperCase[this._rand.Next(0, UpperCase.Length)];
			else
				return LowerCase[this._rand.Next(0, LowerCase.Length)];
		}

		/// <summary>Gets the next random int from the generator</summary>
		/// <returns></returns>
		public int NextRandom(int minValue, int maxValue)
		{
			return this._rand.Next(minValue, maxValue);
		}

		/// <summary>Ranomly replaces a single char in the string with a randomly picked one</summary>
		public string RandomReplace(string str)
		{
			int idx = this.NextRandom(0, str.Length);
			StringBuilder builder = new StringBuilder(str);
			builder[idx] = this.RandomChar();
			return builder.ToString();
		}

		/// <summary>Generates a completly random set of characters of the desired length</summary>
		/// <remarks>Mainly for testing.</remarks>
		public string GenerateCompleteRandom( int len = 5 )
		{
			len = ( len < 0 ) ? 5 : len;

			char[] current = new char[len];

			for ( int i = 0; i < current.Length; ++i )
			{
				current[i] = this.RandomChar();
			}

			return new String( current ).ToUpper();
		}

		/// <summary>Any string less than the min length is padded with random chars</summary>
		public string PadMinLength(string str, int minLen = 3)
		{
			if (str.Length >= minLen)
				return str;

			// concate with some random chars
			return String.Format("{0}{1}", str, this.GenerateCompleteRandom(minLen - str.Length));
		}

	}
}
