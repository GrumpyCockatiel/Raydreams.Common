using System;
using Raydreams.Common.Extensions;
using System.Text;
using Raydreams.Common.Model;

namespace Raydreams.Common.Logging
{
	/// <summary>Console logger</summary>
	public class ConsoleLogger : ILogger
	{
		#region [ Fields ]
		private LogLevel _level = LogLevel.Off;
		private string _src = null;

		#endregion [ Fields ]

		#region [ Constructors ]

		/// <summary></summary>
		/// <param name="source">Who is doing the logging.</param>
		public ConsoleLogger( string source ) : this(source, LogLevel.Off)
		{
		}

		/// <summary></summary>
		/// <param name="source">Who is doing the logging.</param>
		/// <param name="baseLevel">Minimum level to log.</param>
		public ConsoleLogger( string source, LogLevel baseLevel )
		{
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
		protected int InsertLog( string logger, LogLevel lvl, string category, string msg, params object[] args )
		{
			StringBuilder sb = new StringBuilder( DateTime.UtcNow.ToString( "s" ) );
			sb.Append( "|" );

			if ( lvl < this.Level )
				return 0;

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

			Console.WriteLine(sb.ToString());

			return 1;
		}

		#endregion [ Methods ]
	}
}
