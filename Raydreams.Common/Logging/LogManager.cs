using System;
using System.Collections.Generic;
using System.Linq;

namespace Raydreams.Common.Logging
{
	/// <summary>Defines where a log is routed to</summary>
	/// <remarks>Not yet used.</remarks>
	public enum LogTarget
	{
		Unspecified = 0,
		Console = 1,
		Database = 2,
		File = 3,
		Email = 4
	}

	/// <summary>Create a log manager in which to then add Logging Targets. The Log Manager routes all log request</summary>
	/// <example>
	/// LogManager mgr = new LogManager( defaultLogger )
	/// mgr.AddTarget( "Audit", new FileLogger( appName, filePath, LogLevel.Info ) );
	/// </example>
	public class LogManager
	{
		#region [ Categories ]

		/// <summary>The default category name to use if there is none for an exception</summary>
		public static readonly string ErrorCategory = "Exception";

		/// <summary>Default category to specify informative run information like starting and stopping.</summary>
		public static readonly string RunCategory = "RunInfo";

		#endregion [ Categories ]

		/// <summary>Logging routes</summary>
		private Dictionary<string, List<ILogger>> _routes = null;

		/// <summary>The default logger</summary>
		private List<ILogger> _none = null;

		/// <summary>The default source to set if none is specified</summary>
		private string _src = null;

		#region [ Constructors ]

		/// <summary></summary>
		/// <param name="none">The default logger to use if no category route is found.</param>
		public LogManager( ILogger none )
		{
			this._none = new List<ILogger>() { none };
			this._routes = new Dictionary<string, List<ILogger>>();
		}

		#endregion [ Constructors ]

		#region [ Properties ]

		/// <summary></summary>
		/// <remarks>Only added for now to satisfy ILogger interface</remarks>
		public LogLevel Level { get; set; } = LogLevel.All;

		/// <summary>The default logger to use when no Category is defined.</summary>
		public List<ILogger> Defaults
		{
			get { return this._none; }
		}

		/// <summary>Set the logging source. Who is doing the logging.</summary>
		public string Source
		{
			get { return this._src; }
			set
			{
				if ( value != null )
					this._src = value.Trim();
			}
		}

		/// <summary>Gets all the loggers associated with a specific category.</summary>
		public List<ILogger> this[string category]
		{
			get
			{
				return this._routes[category];
			}
		}

		#endregion [ Properties ]

		#region [ Methods ]

		/// <summary>Adds another default logger for logging to multiple places all the time ignoring category</summary>
		public LogManager AddDefault( ILogger logger )
		{
			if ( logger == null )
				return this;

			if ( this._none == null )
				this._none = new List<ILogger>();

			this._none.Add( logger );

			return this;
		}

		/// <summary>Add a logging target to the specified category</summary>
		/// <param name="category">Category to add to the specified logger that determines where the log is routed to</param>
		public LogManager AddTarget( string category, ILogger logger )
		{
			if ( String.IsNullOrWhiteSpace( category ) )
				return this;

			return this.AddTarget( new string[] { category }, logger );
		}

		/// <summary>Add several categories to the same target</summary>
		/// /// <param name="categories">Categories to add to the specified logger</param>
		public LogManager AddTarget(string[] categories, ILogger logger)
		{
			if ( logger == null )
				return this;

			foreach ( string str in categories )
			{
				if ( String.IsNullOrWhiteSpace( str ) )
					continue;

				string cat = str.Trim();

				if ( this._routes.ContainsKey( cat ) )
					this._routes[cat].Add( logger );
				else
					this._routes.Add( cat, new List<ILogger>() { logger } );
			}

			return this;
		}

		/// <summary>Right now FLush has to be called to force the email logger to send out an email. In the future other final clean-up could be done</summary>
		/// <remarks>Ideally it would be cool if flush were called when the class unloads or the app exits.</remarks>
		public void Flush()
		{
			List<EmailLogger> emailers = new List<EmailLogger>();

			// send out the email for any email loggers
			foreach ( KeyValuePair<string, List<ILogger>> pair in this._routes )
			{
				foreach ( ILogger logger in this._routes[pair.Key] )
				{
					// only email loggers not already in the list
					if ( logger is EmailLogger && !emailers.Contains( logger ) )
						emailers.Add( logger as EmailLogger);
				}
			}

			emailers.ForEach( t => t.Send() );
		}

		/// <summary></summary>
		public void Debug( string message )
		{
			if ( this.Defaults != null )
				foreach ( ILogger logger in this.Defaults )
					logger.Debug( message );
		}

		/// <summary>Log a simple message to the default logger</summary>
		public void Log( string message, LogLevel level = LogLevel.Info )
		{
			if ( this.Defaults != null )
				foreach ( ILogger logger in this.Defaults )
					logger.Log( message, level );
		}

		/// <summary>Log a simple message to the default logger</summary>
		public void Log( string message, LogLevel level = LogLevel.Info, params object[] args )
		{
			if ( this.Defaults != null )
				foreach ( ILogger logger in this.Defaults )
					logger.Log( message, null, level, args );
		}

		/// <summary>Log to a specified logger with category</summary>
		public void Log( string message, string category, LogLevel level = LogLevel.Info )
		{
			// if null category
			if ( String.IsNullOrWhiteSpace( category ) )
				this.Log( message, level );
			else
			{
				category = category.Trim();

				if ( this.Defaults != null )
					foreach ( ILogger logger in this.Defaults )
						logger.Log( message, category, level );

				if ( this._routes.ContainsKey( category ) )
					foreach ( ILogger logger in this._routes[category] )
						logger.Log( message, category, level );
			}
		}

		/// <summary>Log to a specified logger with category</summary>
		public void Log( string message, string category, LogLevel level, params object[] args )
		{
			if ( String.IsNullOrWhiteSpace( category ) )
				this.Log( message, level, args );
			else
			{
				category = category.Trim();

				if ( this.Defaults != null )
					foreach ( ILogger logger in this.Defaults )
						logger.Log( message, category, level, args );

				if ( this._routes.ContainsKey( category ) )
					foreach ( ILogger logger in this._routes[category] )
						logger.Log( message, category, level, args );
			}
		}

		/// <summary>Log an exception to the default logger</summary>
		public void Log( Exception exp )
		{
			if ( this.Defaults != null )
				foreach ( ILogger logger in this.Defaults )
					logger.Log( exp );
		}

		/// <summary>Log an exception to the default logger</summary>
		public void Log( Exception exp, params object[] args )
		{
			if ( this.Defaults != null )
				foreach ( ILogger logger in this.Defaults )
					logger.Log( exp, args );
		}

		#endregion [ Methods ]
	}
}
