using Raydreams.Common.Extensions;
using Raydreams.Common.Model;
using System;

namespace Raydreams.Common.Logging
{
	/// <summary>Generic logging interface for all loggers to implement.</summary>
	public interface ILogger
	{
		/// <summary>The minimum logging this logger will accept</summary>
		LogLevel Level { set; }

		/// <summary>Insert a type Debug log</summary>
		void Debug( string message );

		/// <summary>Log a full log object</summary>
		void Log( LogRecord message );

		/// <summary>Log to the default logger</summary>
		void Log( string message, LogLevel level = LogLevel.Info );

		/// <summary>Log to a specific category</summary>
		void Log( string message, string category, LogLevel level = LogLevel.Info );

		/// <summary>Log to a category with additional info</summary>
		void Log( string message, string category, LogLevel level, params object[] args );

		/// <summary>Log an exception as type Error</summary>
		void Log( Exception exception );

		/// <summary>Log an exception as type Error and additional args</summary>
		void Log( Exception exp, params object[] args );
	}

	///// <summary>A log object that holds all the parameters of a standard log message</summary>
	//public class Log
	//{
	//	public Log( LogLevel level )
	//	{
	//		this.Level = level;
	//	}

	//	public Log() : this(LogLevel.Info)
	//	{
	//	}

	//	public string Message { get; set; }

	//	public string Category { get; set; }

	//	public LogLevel Level { get; set; }

	//	public object[] Args { get; set; }
	//}

	/// <summary>Converts an exception to a log object</summary>
	public class ExceptionLog : LogRecord
	{
		public ExceptionLog(System.Exception exp) : base(LogLevel.Error)
		{
			this.Exception = exp;
			this.Message = exp.ToLogMsg( true );
			this.Category = LogManager.ErrorCategory;
		}

		public System.Exception Exception { get; set; }
	}

}
