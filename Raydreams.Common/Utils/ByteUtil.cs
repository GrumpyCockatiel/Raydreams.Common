using System;
using System.Linq;

namespace Raydreams.Common.Utils
{
	/// <summary></summary>
	public static class ByteUtil
	{
		/// <summary>Turns two bytes back to a Short</summary>
		/// <param name="b1"></param>
		/// <param name="b2"></param>
		/// <returns></returns>
		public static ushort BytesToShort( byte b1, byte b2 )
		{
			ushort end = b1;
			end = Convert.ToUInt16( end << 8 );
			end = Convert.ToUInt16( end | b2 );

			return end;
		}

		/// <summary>Converts an array of bytes back to an Int32</summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static int BytesToInt( byte[] bytes )
		{
			int end = bytes[0];
			end = end << 8;
			end = end | bytes[1];
			end = end << 8;
			end = end | bytes[2];
			end = end << 8;
			end = end | bytes[3];

			return end;
		}

		/// <summary></summary>
        /// <param name="n"></param>
        /// <returns></returns>
		public static byte[] IntsToBytes( int[] n )
        {
			// return empty array on invalid input
			if ( n == null || n.Length < 1 )
				return new byte[] { };

			int size = sizeof( int );

			// alloc
			byte[] results = new byte[n.Length * size];

			// iterate from the end of the input
			for ( int i = 0; i < n.Length; ++i )
			{
				byte[] temp = BitConverter.GetBytes( n[i] );
				temp.CopyTo( results, i * size );
			}

			return results;
		}

		/// <summary></summary>
        /// <param name="b"></param>
        /// <returns></returns>
		public static int[] BytesToInts( byte[] b )
		{
			if ( b == null || b.Length < 1 )
				return new int[] { };

			// get the size of the data type
			int size = sizeof( int );

			// should be divisible by 4
			// TODO - test with an uneven byte length
			int[] n = new int[b.Length / size];

			for ( int i = 0; i < n.Length; ++i )
			{
				// set the starting location to read from
				int j = i * size;
				// read next double from the byte array
				int v = BitConverter.ToInt32( b, j );
				n[i] = v;
			}

			return n;
		}

		/// <summary>Serializes an array of doubles into a byte array for binary storage</summary>
		/// <param name="d"></param>
		/// <returns></returns>
		/// <remarks>Inefficient since it is constantly reallocating</remarks>
		public static byte[] DoublesToBytes(double[] d)
        {
			//byte[] bytes = new byte[d.Length * 8];
			byte[] bytes = new byte[0];

			for (int i = 0; i < d.Length; ++i )
            {
				byte[] temp = BitConverter.GetBytes( d[i] );
				bytes = bytes.Concat(temp).ToArray();
			}

			return bytes.ToArray();
		}

		/// <summary>Deserializes a byte array back to a double array</summary>
        /// <param name="b"></param>
        /// <returns></returns>
		public static double[] BytesToDoubles( byte[] b )
        {
			if ( b == null || b.Length < 1 )
				return new double[] { };

			// get the size of the data type
			int size = sizeof( double );

			// should be divisible by 8
			// TODO - test with an uneven byte length
			double[] d = new double[b.Length / size];

			for ( int i = 0; i < d.Length; ++i )
            {
				// set the starting location to read from
				int j = i * size;
				// read next double from the byte array
				double v = BitConverter.ToDouble( b, j );
				d[i] = v;
			}

			return d;
		}


	}
}
