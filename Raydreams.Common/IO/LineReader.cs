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
		Comma = 1,
		Tab = 2,
		Semicolon =3
	}

	/// <summary>Reads text file and returns each line as an array</summary>
	public class LineReader
	{
		#region [ Fields ]

		private StreamReader _reader = null;

		#endregion [ Fields ]

		#region [ Constructors ]

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>Reads every line into a string array</summary>
		/// <returns>The read.</returns>
		/// <param name="path">Path.</param>
		public List<string> Read( string path )
		{
			List<string> data = new List<string>();

			if ( String.IsNullOrWhiteSpace( path ) )
				return data;

			FileInfo fi = new FileInfo( path );

			if ( !fi.Exists )
				return data;

			using ( this._reader = new StreamReader( path, Encoding.UTF8 ) )
			{
				string next = null;

				while ( ( next = this._reader.ReadLine() ) != null )
				{
					if ( String.IsNullOrWhiteSpace( next ) )
						continue;

					next = next.Trim();

					data.Add( next );
				}
			}

			return data;
		}

		#endregion [ Methods ]
	}
}
