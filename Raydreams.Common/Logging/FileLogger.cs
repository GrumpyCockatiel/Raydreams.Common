using System;
using System.IO;
using Raydreams.Common.Extensions;
using System.Text;
using Raydreams.Common.Model;

namespace Raydreams.Common.Logging
{
	/// <summary>Logs to a physical file</summary>
	public class FileLogger : ILogger
	{
		#region [ Fields ]

		/// <summary></summary>
		private readonly object _fileLock = new object();

		private DirectoryInfo _path = null;

		private LogLevel _level = LogLevel.Off;

		private string _src = null;

		private string _filename = null;

		#endregion [ Fields ]

		#region [ Constructors ]

		/// <summary></summary>
		/// <param name="source">Who is doing the logging.</param>
		/// <param name="path">File path to log to</param>
		/// <param name="baseLevel">Minimum level to log.</param>
		public FileLogger( string source, string path, string filename, LogLevel baseLevel = LogLevel.All )
		{
			this._path = new DirectoryInfo( path );
			this.Level = baseLevel;
			this.Source = source;
			this.LogFilename = filename;
		}

		/// <summary></summary>
		public FileLogger( string source, string path, LogLevel baseLevel = LogLevel.All )
		{
			this._path = new DirectoryInfo( path );
			this.Level = baseLevel;
			this.Source = source;
		}

		#endregion [ Constructors ]

		#region [ Properties ]

		/// <summary>When true, if the path does not exist, it will be created.
		/// If false, and the path does not exist, not log will be written.
		/// </summary>
		public bool Create { get; set; } = true;

		/// <summary>The name of the log file</summary>
		public string LogFilename
		{
			get
			{
				return ( String.IsNullOrWhiteSpace( this._filename ) ) ? DefaultFilename : this._filename;
			}
			set { this._filename = value; }
		}

		/// <summary>The minimum level inclusive to log based on the LogLevel enumeration [level,...]</summary>
		public LogLevel Level
		{
			get { return this._level; }
			set { this._level = value; }
		}

		/// <summary>The directory to log to</summary>
		/// <remarks>Very bad name for this since it conflicts with System.IO.Path</remarks>
		public DirectoryInfo Path
		{
			get { return this._path; }
		}

		/// <summary>A fallback filename if none is provided</summary>
		public string DefaultFilename => String.Format( "{0}_log_{1}.txt", this._src, DateTime.UtcNow.ToString( "yyyyMMdd" ) );

		/// <summary>The logging source. Who is doing the logging.</summary>
		public string Source
		{
			get { return this._src; }
			set
			{
				if ( value != null )
					this._src = value.Trim();
			}
		}

		#endregion [ Properties ]

		#region [ Methods ]

		/// <summary></summary>
		/// <param name="message"></param>
		public void Debug( string message )
		{
			this.InsertLog( this.Source, LogLevel.Debug, null, message, null );
		}

        /// <summary></summary>
        /// <param name="message"></param>
        public void Log( LogRecord message )
        {
            this.InsertLog( this.Source, message.Level, message.Category, message.Message, message.Args );
        }

        /// <summary></summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        public void Log( string message, LogLevel level = LogLevel.Info )
		{
			this.InsertLog( this.Source, level, null, message, null );
		}

		/// <summary></summary>
		/// <param name="message"></param>
		/// <param name="level"></param>
		public void Log( string message, string category, LogLevel level = LogLevel.Info )
		{
			this.InsertLog( this.Source, level, category, message, null );
		}

		/// <summary></summary>
		/// <param name="message"></param>
		/// <param name="level"></param>
		/// <param name="args"></param>
		public void Log( string message, string category, LogLevel level, params object[] args )
		{
			this.InsertLog( this.Source, level, category, message, args );
		}

		/// <summary></summary>
		/// <param name="exp"></param>
		public void Log( System.Exception exp )
		{
			this.InsertLog( this.Source, LogLevel.Error, LogManager.ErrorCategory, exp.ToLogMsg( true ), null );
		}

		/// <summary></summary>
		/// <param name="exp"></param>
		public void Log( Exception exp, params object[] args )
		{
			this.InsertLog( this.Source, LogLevel.Error, LogManager.ErrorCategory, exp.ToLogMsg( true ), args );
		}

		/// <summary>Base logging method</summary>
		/// <param name="logger">The source of the source such as the application name.</param>
		/// <param name="lvl">The std log level as defined by Log4j</param>
		/// <param name="category">An application specific category that can be used for further organization, or routing to differnt locations/</param>
		/// <param name="msg">The actual message to log</param>
		/// <param name="args">any additional data fields to append to the log message. Used for debugging.</param>
		/// <returns></returns>
		protected bool InsertLog( string logger, LogLevel lvl, string category, string msg, params object[] args )
		{
			StringBuilder sb = new StringBuilder( DateTime.UtcNow.ToString( "s" ) );
			sb.Append( "|" );

			if ( lvl < this.Level )
				return false;

			// make sure the parent dir exists
			if ( !this.Path.Exists )
			{
				if ( this.Create )
					Directory.CreateDirectory( this.Path.FullName );
				else
					return false;
			}

			// construct a full file path
			string fullpath = System.IO.Path.Combine( this.Path.FullName, this.LogFilename );

			// append level
			sb.AppendFormat( "{0}|", lvl );

			// append category
			if ( String.IsNullOrWhiteSpace( category ) )
				sb.Append( "<none>|" );
			else
				sb.AppendFormat( "{0}|", category );

			if ( !String.IsNullOrWhiteSpace( msg ) )
				sb.AppendFormat( "{0}", msg.Trim() );

			// convert the args dictionary to a string and add to the end
			if ( args != null && args.Length > 0 )
				sb.AppendFormat( "|args={0}", String.Join( ";", args ) );

			// write log
			lock ( _fileLock )
			{
				StreamWriter osw = null;

				try
				{
					// open file
					using ( osw = new StreamWriter( fullpath, true ) )
					{
						osw.WriteLine( sb.ToString() );
					}
				}
				catch ( System.Exception exp )
				{
					throw exp;
				}
				finally
				{
					if ( osw != null )
						osw.Close();
				}
			}

			return true;
		}

		#endregion [ Methods ]

	}
}
