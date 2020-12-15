using Raydreams.Common.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Raydreams.Common.IO
{
	/// <summary>Writes FieldMap adorned objects to a file</summary>
	public class DataFileWriter
	{
		#region [ Fields ]

		private StreamWriter _osw = null;
		private string _filePath = null;
		private string _colDelim = ",";
		private string _newline = null;
		private string[] _headers = null;
		private char _fieldChar = Char.MinValue;

		#endregion [ Fields ]

		#region [ Constructors ]

		/// <summary></summary>
		/// <param name="path">Complete path including filename to write to</param>
		public DataFileWriter( string path )
		{
			this._filePath = path.Trim();
			//this.UseNewLine = true;
		}

		#endregion [ Constructors ]

		#region [ Properties ]

		/// <summary>String between each column</summary>
		public string ColDelimitor
		{
			get { return this._colDelim; }
			set { this._colDelim = value.Trim(); }
		}

		/// <summary>string at the end of a row if something other than an environment newline</summary>
		public string RowDelimitor
		{
			get { return this._newline; }
			set { this._newline = value.Trim(); }
		}

		/// <summary>Get the path being written to</summary>
		public string FilePath
		{
			get { return this._filePath; }
		}

		/// <summary>Field quote character to use if any</summary>
		public char FieldQuoteChar
		{
			get { return this._fieldChar; }
			set { this._fieldChar = value; }
		}

		/// <summary>Writes a newline character at the end of each row if true</summary>
		//public bool UseNewLine { get; set; }

		#endregion [ Properties ]

		#region [ Methods ]

		/// <summary>Static convenience method to delete a directory</summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static void DeleteDirectory( string path )
		{
			if ( !String.IsNullOrWhiteSpace( path ) && Directory.Exists( path ) )
				Directory.Delete( path, true );
		}

		/// <summary>Static convenience method to create a directory</summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static DirectoryInfo CreateDirectory( string path )
		{
			if ( String.IsNullOrWhiteSpace( path ) )
				return null;

			return System.IO.Directory.CreateDirectory( path );
		}

		/// <summary>Open the output file</summary>
		public bool Open()
		{
			FileInfo fi = new FileInfo( this.FilePath );

			this._osw = new System.IO.StreamWriter( this.FilePath, false );

			return ( this._osw != null );
		}

		/// <summary>Writes the list of headers to the file</summary>
		public int WriteHeader( IEnumerable<string> headers )
		{
			int cols = 0;

			if ( headers == null )
				return cols;

			// keep the headers
			this._headers = headers.ToArray<string>();

			StringBuilder sb = new StringBuilder();

			foreach ( string s in this._headers )
			{
				if ( this.FieldQuoteChar == Char.MinValue )
					sb.AppendFormat( "{0}{1}", s, this.ColDelimitor );
				else
					sb.AppendFormat( "{2}{0}{2}{1}", s, this.ColDelimitor, this.FieldQuoteChar );
				++cols;
			}

			--sb.Length;

			if ( !String.IsNullOrWhiteSpace( this.RowDelimitor ) )
				sb.Append( this.RowDelimitor );

			this._osw.WriteLine( sb.ToString() );

			return cols;
		}

		/// <summary>Writes a single line to the file.</summary>
		/// <param name="line">The string to write.</param>
		/// <returns></returns>
		public void WriteRawLine( string line )
		{
			if ( line == null )
				return;

			this._osw.WriteLine( line );
		}

		/// <summary>Writes the given string list to a delimited line</summary>
		/// <param name="values"></param>
		public void WriteValuesToLine( params string[] values )
		{
			StringBuilder sb = new StringBuilder();

			foreach ( string s in values )
			{
				if ( this.FieldQuoteChar == Char.MinValue )
					sb.AppendFormat( "{0}{1}", ( s == null ) ? String.Empty : s, this.ColDelimitor );
				else
					sb.AppendFormat( "{2}{0}{2}{1}", ( s == null ) ? String.Empty : s, this.ColDelimitor, this.FieldQuoteChar );
			}

			sb.Length = sb.Length - this.ColDelimitor.Length;

			if ( !String.IsNullOrWhiteSpace( this.RowDelimitor ) )
				sb.Append( this.RowDelimitor );

			this._osw.WriteLine( sb.ToString() );
		}

		/// <summary>Write an objects values to the file stream</summary>
		/// <param name="obj"></param>
		public bool Write<T>( T obj, string context = null )
		{
			// if headers are used then
			if ( this._headers != null && this._headers.Length > 0 )
				return this.WriteWithHeaders<T>( obj, context );

			// else without headers
			this.WriteWithoutHeaders<T>( obj, context );

			return true;
		}

		/// <summary>Write an objects values to the file stream</summary>
		/// <param name="obj"></param>
		private bool WriteWithoutHeaders<T>( T obj, string context )
		{
			if ( obj == null )
				return false;

			// get all the properties in the class
			PropertyInfo[] props = typeof( T ).GetProperties( BindingFlags.Public | BindingFlags.Instance );

			if ( props.Length < 1 )
				return false;

			// get all the fields marked with a destination, if there are NO destinations, then assume we want to write EVERY field
			List<string> destinations = RayPropertyAttribute.GetDestinations( typeof( T ) );

			StringBuilder sb = new StringBuilder();

			// get the property info
			foreach ( PropertyInfo property in props )
			{
				if ( !property.CanRead )
					continue;

				object value = null;

				if ( String.IsNullOrWhiteSpace( context ) )
				{
					value = property.GetValue( obj );
				}
				else
				{
					// get the properties FieldMap attribute
					List<RayPropertyAttribute> dests = property.GetCustomAttributes<RayPropertyAttribute>( false ).ToList();

					// if there is not field map source, then this property is not read from the CSV
					if ( dests == null || dests.Count < 1 )
						continue;

					RayPropertyAttribute map = dests.Where( s => s.Context.Equals( context, StringComparison.Ordinal ) ).FirstOrDefault();

					if ( map == null || String.IsNullOrWhiteSpace( map.Destination ) )
						continue;

					value = property.GetValue( obj );
				}

				// write to the file
				if ( this.FieldQuoteChar == Char.MinValue )
					sb.AppendFormat( "{0}{1}", ( value == null ) ? String.Empty : value.ToString(), this.ColDelimitor );
				else
					sb.AppendFormat( "{2}{0}{2}{1}", ( value == null ) ? String.Empty : value.ToString(), this.ColDelimitor, this.FieldQuoteChar );
			}

			// trim
			sb.Length = sb.Length - this.ColDelimitor.Length;

			if ( !String.IsNullOrWhiteSpace( this.RowDelimitor ) )
				sb.Append( this.RowDelimitor );

			this._osw.WriteLine( sb.ToString() );

			return true;
		}

		/// <summary>Write an objects values to the file stream using the headers to specify which ones and what order</summary>
		/// <param name="obj"></param>
		private bool WriteWithHeaders<T>( T obj, string context )
		{
			if ( obj == null )
				return false;

			// get all the properties in the class
			PropertyInfo[] props = typeof( T ).GetProperties( BindingFlags.Public | BindingFlags.Instance );

			if ( props.Length < 1 )
				return false;

			StringBuilder sb = new StringBuilder();

			// loop through the headers and find the matching property destination
			foreach ( string header in this._headers )
			{
				object value = null;

				// find a matching property with a FieldDestination value equal to this header
				foreach ( PropertyInfo property in props )
				{
					// can you even read this property
					if ( !property.CanRead )
						continue;

					// get the properties FieldDestination attribute
					List<FieldDestinationAttribute> dests = property.GetCustomAttributes<FieldDestinationAttribute>( false ).ToList();

					// if there is not field dest, then this property is not written
					if ( dests == null || dests.Count < 1 )
						continue;

					// if no specified context then use a Destination with NO Context
					if ( String.IsNullOrWhiteSpace( context ) )
					{
						FieldDestinationAttribute map = dests.Where( s => String.IsNullOrWhiteSpace( s.Context )
							&& !String.IsNullOrWhiteSpace( s.Destination )
							&& s.Destination.Equals( header, StringComparison.InvariantCulture ) ).FirstOrDefault();

						if ( map == null )
							continue;

						value = property.GetValue( obj );
					}
					else // is a context
					{
						FieldDestinationAttribute map = dests.Where( s => !String.IsNullOrWhiteSpace( s.Context )
							&& !String.IsNullOrWhiteSpace( s.Destination )
							&& s.Context.Equals( context, StringComparison.InvariantCulture )
							&& s.Destination.Equals( header, StringComparison.InvariantCulture ) ).FirstOrDefault();

						if ( map == null )
							continue;

						value = property.GetValue( obj );
					}

				}

				if ( this.FieldQuoteChar == Char.MinValue )
					sb.AppendFormat( "{0}{1}", ( value == null ) ? String.Empty : value.ToString(), this.ColDelimitor );
				else
					sb.AppendFormat( "{2}{0}{2}{1}", ( value == null ) ? String.Empty : value.ToString(), this.ColDelimitor, this.FieldQuoteChar );

			}

			sb.Length = sb.Length - this.ColDelimitor.Length;

			if ( !String.IsNullOrWhiteSpace( this.RowDelimitor ) )
				sb.Append( this.RowDelimitor );

			this._osw.WriteLine( sb.ToString() );

			return true;
		}

		/// <summary>Close the file stream</summary>
		public void Close()
		{
			if ( this._osw != null )
			{
				this._osw.Flush();
				this._osw.Close();
			}
		}

		#endregion [ Methods ]

	}
}
