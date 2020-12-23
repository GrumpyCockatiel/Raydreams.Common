using Raydreams.Common.Data;
using Raydreams.Common.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Raydreams.Common.IO
{
	/// <summary>Delegate for handling a record being read</summary>
	public delegate int ReadRecord<T>( object sender, ReadEventArgs<T> e );

	/// <summary>Event handler when a record is successfully read.</summary>
	public class ReadEventArgs<T> : System.EventArgs
	{
		private T _obj = default( T );

		public ReadEventArgs() { }

		// tha parsed object
		public T Item { get; set; }

		// the original records
		public Dictionary<string, string> Records { get; set; }
	}

	/// <summary>Uses FieldSource</summary>
	/// <typeparam name="T">The object class type that maps to each record.</typeparam>
	public class DataFileReader<T> where T : class, new()
	{
		#region [ Fields ]

		public StreamReader _reader = null;

		#endregion [ Fields ]

		#region [ Constructors ]

		/// <summary>Constructor</summary>
		/// <param name="parser">The line parser to use to split a string into fields. Usually CSV</param>
		public DataFileReader( TextLineParser parser )
		{
			this.LineReader = parser;
		}

		#endregion [ Constructors ]

		#region [ Properties ]

		/// <summary>The line parsing function</summary>
		public TextLineParser LineReader { get; set; }

		/// <summary>Event handler for after a line is read</summary>
		public event ReadRecord<T> ReadCSVLine;

		#endregion [ Properties ]

		#region [ Methods ]

		/// <summary>Broadcast record read with its object value</summary>
		/// <param name="e"></param>
		protected virtual void OnNewLineRead( ReadEventArgs<T> e )
		{
			if ( this.ReadCSVLine != null )
			{
				_ = this.ReadCSVLine( this, e );
			}
		}

		/// <summary>Read an entire file from the specified path</summary>
		/// <param name="path">Full file path to the CSV file to read</param>
        /// <param name="context">Use a specific context specified on the object properties</param>
		/// <param name="hasHeader">Is the first line a header record</param>
		/// <returns>A list of object T</returns>
		public List<T> Read( string path, string context = null, bool hasHeader = true )
		{
			// start a new list
			List<T> data = new List<T>();

			// validate the file path
			if ( String.IsNullOrWhiteSpace( path ) )
				return data;

			// get a handle to the file
			FileInfo fi = new FileInfo( path );

			if ( !fi.Exists )
				return data;

			// open a reader
			using ( this._reader = new StreamReader( path, Encoding.UTF8 ) )
			{
				// get the header row
				string first = this._reader.ReadLine();
				string[] headers = this.LineReader( first );

				string next = null;

				// read each line
				while ( ( next = this._reader.ReadLine() ) != null )
				{
					// read this row
					string[] values = this.LineReader( next );

					// make a new array to copy the values into that is the length of the headers
					string[] valuesCopy = new string[headers.Length];

					// so long as the values length is less than or equal to the headers length
					if ( values.Length <= valuesCopy.Length )
						values.CopyTo( valuesCopy, 0 );
					else
					{
						// log this somehow
						continue;
					}

					// create a new dictionary
					Dictionary<string, string> rec = new Dictionary<string, string>();

					// add all the values to a dictionary
					for ( int i = 0; i < headers.Length; ++i )
						rec.Add( headers[i], valuesCopy[i] );

					// convert the dictionary to an object using the extension
					T obj = DictionaryExtensions.SourceToObject<T>( rec, context );

					// send the data to listeners along with the original data
					this.OnNewLineRead( new ReadEventArgs<T>() { Item = obj, Records = rec } );

					if ( obj != null )
						data.Add( obj );
				}
			}

			return data;
		}

		#endregion [ Methods ]
	}
}
