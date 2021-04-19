using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Raydreams.Common.Logic
{
	/// <summary>Defines the combinations of characters to use</summary>
	[FlagsAttribute]
	public enum CharSet : short
	{
		Empty = 0,
		Digits = 1,
		Lower = 2,
		Upper = 4,
		LimitedSpecial = 8,
		AllSpecial = 16,
		NoSimilar = 32,
		All = 64
	};

    /// <summary>Class for generating all kinds of random things</summary>
	public class Randomizer
	{
		/// <summary></summary>
        private Random _rand = null;

		#region [String Sets]

		public static readonly char[] LowerCase = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
		public static readonly char[] UpperCase = { 'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z' };
        public static readonly char[] Digits = {'0','1','2','3','4','5','6','7','8','9'};
        public static readonly char[] MoreSpecial = { '"', '\'', '(', ')', ',', '-', '.', '/', ':', ';', '<', '=', '>', '[', '\\', ']', '^', '_', '`', '{', '|', '}'};
		public static readonly char[] LimitedSpecial = { '!','~', '*', '@', '#', '$', '%', '+', '?', '&' };
		public static readonly char[] Similar = { '0', 'O', 'o', '1', 'l', 'L', 'i', 'I', '!', '\'', '`', '"' };

		#endregion [String Sets]

		#region [Constructors]

		/// <summary>Init with some already created Random instance.</summary>
		/// <param name="generator"></param>
		/// <remarks></remarks>
		public Randomizer(Random generator)
        {
            this._rand = generator ?? this.InitRandom();
        }

        /// <summary>Generate the random generator in the class</summary>
        public Randomizer()
		{
            this._rand = this.InitRandom();
		}

		/// <summary>Init a random generator</summary>
		/// <remarks>
        /// There are multiple ways to generate a seed like
		/// new Random( Guid.NewGuid().GetHashCode() );
		/// </remarks>
		private Random InitRandom()
        {
			// generate a random seed to size of Int32
			byte[] seed = new byte[4];
            new RNGCryptoServiceProvider().GetBytes(seed);

            // convert to an integer value
            return new Random( BitConverter.ToInt32(seed, 0) );
        }

		#endregion [Constructors]

		/// <summary>Get the random generator being used.</summary>
		public Random Generator
        {
            get { return this._rand; }
        }

        /// <summary>Gets the next random int from the generator calling Next where the max value is not included [min,max)</summary>
        /// <returns></returns>
        protected int NextRandom(int minValue, int maxValue)
        {
            return this._rand.Next(minValue, maxValue);
        }

		/// <summary>Generates a list of characters to choose from the specified sets</summary>
		/// <param name="set"></param>
		/// <returns></returns>
		protected char[] MakeCharSet(CharSet set = CharSet.Empty)
		{
			List<char> all = new List<char>();

			// add in digits
			if ( (set & CharSet.Digits) == CharSet.Digits )
				all.AddRange( Digits );

			// add in lower case
			if ( (set & CharSet.Lower) == CharSet.Lower )
				all.AddRange( LowerCase );

			// add in upper case
			if ( (set & CharSet.Upper) == CharSet.Upper )
				all.AddRange( UpperCase );

			// add in all special chars
			// if both limited and all are specified - then its still all
			if ( (set & CharSet.AllSpecial) == CharSet.AllSpecial )
			{
				all.AddRange( LimitedSpecial );
				all.AddRange( MoreSpecial );
			}
			else if ( (set & CharSet.LimitedSpecial) == CharSet.LimitedSpecial )
				all.AddRange( LimitedSpecial );

			// if at this point it's empty then nothing was specified

			// remove similars
			if ( all.Count > 0 && (set & CharSet.NoSimilar) == CharSet.NoSimilar )
			{
				return all.Except( Similar ).ToArray();
			}

			return all.ToArray();
		}

		/// <summary>Pick a random integer between [low, high] inclusive on both ends.</summary>
		/// <param name="high">Highest possible includsive value</param>
		/// <param name="low">Lowest possible includsive value</param>
		public int RandomInt(int low, int high)
		{
			// answer is obvious
			if (low == high)
				return high;

			// swap if necessary
			if (low > high)
			{
				int temp = high;
				high = low;
				low = temp;
			}

			return this._rand.Next(low, high + 1);
		}

		/// <summary>Returns a random double from 0 to the max value</summary>
		/// <param name="max">Multiples a 0-1 random double to the max value so a 1 is still 0-1</param>
		/// <returns></returns>
		public double RandomDouble(double max = 1)
		{
			if (max < 0)
				max = 0;

			return max * this._rand.NextDouble();
		}

		/// <summary>Generate some random bytes</summary>
		/// <remarks>Has a 1024 max limit for now</remarks>
		public byte[] RandomBytes( int size )
		{
			if ( size < 1 )
				size = 1;

			if ( size > 1024 )
				size = 1024;

			byte[] b = new byte[size];
			this._rand.NextBytes( b );
			return b;
		}

		/// <summary>Chooses random points in a 2D matrix</summary>
		/// <param name="width">Width of the view in pixels</param>
		/// <param name="height">Height of the view in pixels</param>
		/// <param name="density">Percent of pixels [1%-100%].</param>
		/// <returns>Array of randomly chosen points</returns>
		/// <remarks>
		/// Star density works best between .001 to .01 (.1% to 1%)
		/// </remarks>
		public Point[] RandomPoints(int width, int height, double density)
		{
			if (density <= 0.0)
				return new Point[0];

			int pixels = (int)Math.Floor(width * height * density);
			Point[] pts = new Point[pixels];

			for ( int i = 0; i < pts.Length; ++i)
            {
				int x = this.NextRandom( 0, width );
				int y = this.NextRandom( 0, height );
				pts[i] = new Point(x,y);
			}

			return pts;
		}

		/// <summary>Pick a single ramdom character</summary>
		/// <returns></returns>
		public char RandomChar(CharSet set)
		{
			char[] chars = this.MakeCharSet(set);

			return chars[this._rand.Next(0, chars.Length)];
		}

		/// <summary>Generates a random character code using upper and lower case and all digits</summary>
		public string RandomCode(int len = 10, CharSet set = CharSet.Lower | CharSet.Upper | CharSet.Digits)
		{
			if (len < 1)
				len = 1;

			if (len > 1024)
				len = 1024;

			char[] chars = this.MakeCharSet(set);

			if ( chars.Length < 1 )
				return String.Empty;

			StringBuilder results = new StringBuilder();

			for (int i = 0; i < len; ++i)
				results.Append(chars[this._rand.Next(0, chars.Length)]);

			return results.ToString();
		}

		/// <summary>Any string less than the min length is padded with random chars</summary>
		public string PadMinLength(string str, int minLen = 3)
        {
            if (str.Length >= minLen)
                return str;

            // concate with some random chars
            return String.Format("{0}{1}", str, this.RandomCode(minLen - str.Length));
        }

        /// <summary>Randomly replaces a single char in the string with a randomly picked one</summary>
        public string RandomReplace(string str)
        {
            int idx = this.NextRandom(0, str.Length);
            StringBuilder builder = new StringBuilder(str);
            builder[idx] = this.RandomChar( CharSet.Lower | CharSet.Upper | CharSet.Digits);
            return builder.ToString();
        }

        /// <summary>A saving roll is the attribute value - the avg + some random value from [0,9]</summary>
        /// <remarks>This is just an example</remarks>
        public int SavingRoll( int attr, int avg )
		{
			return attr - avg + this.RandomInt( 0, 9 );
		}

        /// <summary>Chooses a random angle between 0 and 2Pi</summary>
        /// <returns>A random angle in radians</returns>
        public double RandomAngle()
        {
            return Math.PI * 2.0 * this._rand.NextDouble();
        }

		/// <summary>Generates a completely random color with Alpha = 255</summary>
		public Color RandomColor()
		{
			// the max value is not included in the range
			return Color.FromArgb( this.NextRandom( 0, 256 ), this.NextRandom( 0, 256 ), this.NextRandom( 0, 256 ) );
		}

		/// <summary>Returns one of the random enum value of enumbn type T</summary>
		public T RandomEnum<T>()
		{
			Array values = Enum.GetValues( typeof( T ) );

			int idx = this._rand.Next( values.Length-1 );

			return (T)values.GetValue( idx );
		}

		/// <summary>Creates a random folio word like Zulu22</summary>
		/// <returns></returns>
		public string Folio()
		{
			string[] dictionary = Model.NATO.Values;
			string word = dictionary[this._rand.Next( dictionary.Length )];
			return $"{word}{this.RandomInt( 10, 99 )}";
		}

		/// <summary>Creates a single sentenance of Lorem Ipsum</summary>
		/// <param name="minWords"></param>
		/// <param name="maxWords"></param>
		/// <param name="caseAll"></param>
		/// <returns></returns>
		public string LoremIpsum( int minWords, int maxWords, bool caseFirst = false, bool caseAll = true )
		{
			string[] dictionary = Raydreams.Common.Model.LoremIpsum.Values;

			StringBuilder result = new StringBuilder();

			int words = this.RandomInt( minWords, maxWords );

			for ( int w = 0; w < words; ++w )
			{
				string word = dictionary[this._rand.Next( dictionary.Length )];
				result.Append( word );

				if ( caseAll )
					result[result.Length - word.Length] = Char.ToUpper( word[0] );
				else if (caseFirst && w == 0)
					result[0] = Char.ToUpper( word[0] );

				result.Append( " " );
			}

			--result.Length;
			return result.ToString();
		}

		/// <summary>Creates paragraphs of Lorem Ipsum</summary>
		/// <param name="minWords"></param>
		/// <param name="maxWords"></param>
		/// <param name="minSentences"></param>
		/// <param name="maxSentences"></param>
		/// <param name="paras"></param>
		/// <returns></returns>
		public string LoremIpsumBody( int minWords, int maxWords, int minSentences, int maxSentences, int paras )
		{
			string[] dictionary = Raydreams.Common.Model.LoremIpsum.Values;

			StringBuilder result = new StringBuilder();

			for ( int p = 0; p < paras; ++p )
			{
				result.Append( "<div>" );

				int sentences = this.RandomInt( minSentences, maxSentences );

				for ( int s = 0; s < sentences; ++s )
				{
					int words = this.RandomInt( minWords, maxWords );

					for ( int w = 0; w < words; ++w )
					{
						string word = dictionary[this._rand.Next( dictionary.Length )];
						result.Append( word );

						if ( w == 0 )
							result[result.Length - word.Length] = Char.ToUpper( word[0] );

						result.Append( " " );
					}

					--result.Length;
					result.Append( ". " );
				}

				--result.Length;
				result.Append( "</div>" );
			}

			return result.ToString();
		}

	}

}
