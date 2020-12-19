using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raydreams.Common.Extensions
{
	public static class StreamExtensions
	{
		/// <summary>Writes the characters from an ASCII string into the specified stream</summary>
		/// <param name="stream">The stream to write the data to.</param>
		/// <param name="value">The value to write</param>
		public static void Write(this Stream stream, string value)
		{
			foreach (char c in value)
			{
				stream.WriteByte((byte)c);
			}
		}

		/// <summary>
		/// Writes a floating point number in big-endian format.
		/// </summary>
		/// <param name="stream">The stream to write the data to.</param>
		/// <param name="value">The value to write</param>
		public static void WriteBigEndian(this Stream stream, float value)
		{
			byte[] bytes = BitConverter.GetBytes(value);

			stream.WriteByte(bytes[3]);
			stream.WriteByte(bytes[2]);
			stream.WriteByte(bytes[1]);
			stream.WriteByte(bytes[0]);
		}

		/// <summary>
		/// Writes a unicode string to the specified stream in big-endian format
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="value">The value.</param>
		public static void WriteBigEndian(this Stream stream, string value)
		{
			byte[] data;

			data = Encoding.BigEndianUnicode.GetBytes(value);

			stream.WriteBigEndian((ushort)value.Length);
			stream.Write(data, 0, data.Length);
		}

		/// <summary>
		/// Writes a 16bit unsigned integer in big-endian format.
		/// </summary>
		/// <param name="stream">The stream to write the data to.</param>
		/// <param name="value">The value to write</param>
		public static void WriteBigEndian(this Stream stream, ushort value)
		{
			stream.WriteByte((byte)(value >> 8));
			stream.WriteByte((byte)(value >> 0));
		}

		/// <summary>Writes a 32bit unsigned integer in big-endian format.</summary>
		/// <param name="stream">The stream to write the data to.</param>
		/// <param name="value">The value to write</param>
		public static void WriteBigEndian(this Stream stream, int value)
		{
			stream.WriteByte((byte)((value & 0xFF000000) >> 24));
			stream.WriteByte((byte)((value & 0x00FF0000) >> 16));
			stream.WriteByte((byte)((value & 0x0000FF00) >> 8));
			stream.WriteByte((byte)((value & 0x000000FF) >> 0));
		}
	}
}
