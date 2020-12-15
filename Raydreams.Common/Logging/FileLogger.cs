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

		private StreamWriter _osw = null;
		private DirectoryInfo _path = null;
		private LogLevel _level = LogLevel.Off;
		private string _src = null;

		#endregion [ Fields ]

		#region [ Constructors ]

		/// <summary></summary>
		/// <param name="source">Who is doing the logging.</param>
		/// <param name="path">File path to log to</param>
		public FileLogger( string source, string path ) : this(source, path, LogLevel.Off)
		{
		}

		/// <summary></summary>
		/// <param name="source">Who is doing the logging.</param>
		/// <param name="path">File path to log to</param>
		/// <param name="baseLevel">Minimum level to log.</param>
		public FileLogger( string source, string path, LogLevel baseLevel )
		{
			this._path = new DirectoryInfo( path );
			this.Level = baseLevel;
			this.Source = source;
		}

		#endregion [ Constructors ]

		#region [ Properties ]

		/// <summary>The minimum level inclusive to log based on the LogLevel enumeration [level,...]</summary>
		public LogLevel Level
		{
			get { return this._level; }
			set { this._level = value; }
		}

		/// <summary>The directory to log to</summary>
		public DirectoryInfo Path
		{
			get { return this._path; }
		}

		/// <summary>Generates a log file name to use for today</summary>
		public string DefaultFilename
		{
			get
			{
				return String.Format("{0}_log_{1}.txt", this._src, DateTime.UtcNow.ToString("yyyyMMdd"));
			}
		}

		/// <summary>The logging source. Who is doing the logging.</summary>
		public string Source
		{
			get { return this._src; }
			set
			{
				if (value != null)
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
		public void Log(LogRecord message )
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
		protected int InsertLog( string logger, LogLevel lvl, string category, string msg, params object[] args )
		{
			StringBuilder sb = new StringBuilder( DateTime.UtcNow.ToString("s") );
			sb.Append( "|" );

			if ( lvl < this.Level )
				return 0;

			// make sure the parent dir exists
			if ( !this.Path.Exists )
				return 0;

			// construct a full file path
			string fullpath = this.Path.FullName + this.DefaultFilename;

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

			try
			{
				// open file
				this._osw = new StreamWriter( fullpath, true );

				// write log
				this._osw.WriteLine( sb.ToString() );
			}
			catch ( System.Exception exp )
			{
				throw exp;
			}
			finally
			{
				if ( this._osw != null )
					this._osw.Close();
			}

			return 1;
		}

		#endregion [ Methods ]

	}
}
