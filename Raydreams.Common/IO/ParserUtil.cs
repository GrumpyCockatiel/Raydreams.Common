using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raydreams.Common.IO
{
	/// <summary>Enumerates line end deliminators</summary>
	public enum Delim : int
	{
		Unknown = 0,
		Comma = 1, // CSV
		Tab = 2,
		Semicolon = 3
	}

	/// <summary>Defines a delegate method that can be used to parse a text line</summary>
	/// <remarks>Any function that takes a single line in and returns an array field of strings</remarks>
	public delegate string[] TextLineParser( string line );

	/// <summary>A temp class for holding line parsing functions</summary>
	public static class ParserUtil
	{
		/// <summary>Given a file path, sniffs the target to be either tabbed or CSV deliminated</summary>
		/// <param name="path">Path to a physical file to test</param>
        /// <returns>The format type as an enum</returns>
        /// <remarks>The logic sucks - its just a basic test</remarks>
		public static Delim Sniffer( string path )
		{
			Delim results = Delim.Unknown;

			if ( String.IsNullOrWhiteSpace( path ) )
				return results;

			// check the file exists
			FileInfo fi = new FileInfo( path );

			if ( !fi.Exists )
				return results;

			// read the file
			using ( StreamReader reader = new StreamReader( path, Encoding.UTF8 ) )
			{
				// read the first line
				string next = reader.ReadLine();

				// if there is a first line
				if ( !String.IsNullOrWhiteSpace( next ) )
				{
					// now split
					string[] tabs = next.Split( new char[] { '\t' }, StringSplitOptions.None );
					string[] commas = next.Split( new char[] { ',' }, StringSplitOptions.None );

					results = (tabs.Length > commas.Length) ? Delim.Tab : Delim.Comma;
				}
			}

			return results;
		}

		/// <summary>Processes is a single line comma delimited line using CSV parsing formats</summary>
		/// <param name="line">The single line to process</param>
		/// <returns>The fields as an array of strings</returns>
		/// <remarks></remarks>
		public static string[] CSVLineReader( string line )
		{
			StringBuilder temp = new StringBuilder();
			List<string> values = new List<string>();

			// walk the string
			for (int i = 0; i < line.Length;)
			{
				// if the record starts on a quote, then it ends on the next quote
				if (line[i] == '"')
				{
					++i; // move off the current "

					// continue to the last quote
					while (line[i] != '"')
					{
						temp.Append( line[i++] );
					}

					// found an end
					values.Add( temp.ToString().Trim() );
					temp.Length = 0;
					i += 2;
				}
				else if (line[i] == ' ') // stray space
				{ ++i; }
				else if (line[i] == ',') // found two ,, next to each other or with only space between
				{
					++i;
					values.Add( String.Empty );
				}
				else // non token case, end on the next ,
				{
					// continue to the last ,
					while ( i < line.Length && line[i] != ',')
					{
						temp.Append( line[i++] );
					}

					// found an end
					values.Add( temp.ToString().Trim() );
					temp.Length = 0;

					// look from here to EOL of line and see if there is anthing else
					if ( i == line.Length - 1 && line[i] == ',' )
						values.Add( String.Empty );

					i += 1;
				}
			}

			return values.ToArray();
		}

		/// <summary>Parses a tabbed sinle line to an array of strings</summary>
		/// <param name="line">The single line to process</param>
		/// <returns>The fields as an array of strings</returns>
		public static string[] TabLineReader( string line )
		{
			string[] values = line.Split( new char[] { '\t' }, StringSplitOptions.None );

			Array.ForEach<string>( values, s => s = s.Trim() );

			return values;
		}

	}
}
