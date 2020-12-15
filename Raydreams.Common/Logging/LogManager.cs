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
		/// <summary>The default category name to use if there is none for an exception</summary>
		public static readonly string ErrorCategory = "Exception";
		
		/// <summary>Default category to specify infromative run information like starting and stopping.</summary>
		public static readonly string RunCategory = "RunInfo";

		private Dictionary<string, List<ILogger>> _routes = null;
		private ILogger _none = null;
		private string _src= null;

		#region [ Constructors ]

		/// <summary></summary>
		/// <param name="none">The default logger to use if no cattegory route is found.</param>
		public LogManager( ILogger none )
		{
			this.Default = none;
			this._routes = new Dictionary<string, List<ILogger>>();
		}

		#endregion [ Constructors ]

		#region [ Properties ]

		/// <summary>The default logger to use if no Category is defined.</summary>
		public ILogger Default
		{
			private set { this._none = value; }
			get { return this._none; }
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

		/// <summary>Add a logging target to the specified category</summary>
		/// <param name="category">Category to add to the specified logger</param>
		public void AddTarget( string category, ILogger logger )
		{
			if ( String.IsNullOrWhiteSpace( category ) )
				return;

			this.AddTarget( new string[] { category }, logger );
		}

		/// <summary>Add several categories to the same target</summary>
		/// /// <param name="categories">Categories to add to the specified logger</param>
		public void AddTarget(string[] categories, ILogger logger)
		{
			if ( logger == null )
				return;

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
			if ( this.Default != null )
				this.Default.Debug( message );
		}

		/// <summary></summary>
		public void Log( string message, LogLevel level = LogLevel.Info )
		{
			if ( this.Default != null )
				this.Default.Log( message, level );
		}

		/// <summary></summary>
		public void Log( string message, string category, LogLevel level = LogLevel.Info )
		{
			if ( String.IsNullOrWhiteSpace( category ) )
				this.Default.Log( message, level );
			// no category that exists
			else if ( !this._routes.ContainsKey( category ) )
				this.Default.Log( message, category, level );
			else
			{
				foreach ( ILogger logger in this._routes[category] )
					logger.Log( message, category, level );
			}
		}

		/// <summary></summary>
		public void Log( string message, string category, LogLevel level, params object[] args )
		{
			if ( String.IsNullOrWhiteSpace( category ) )
				this.Default.Log( message, level );
			// no category that exists
			else if ( !this._routes.ContainsKey( category ) )
				this.Default.Log( message, category, level, args );
			else
			{
				foreach ( ILogger logger in this._routes[category] )
					logger.Log( message, category, level, args );
			}
		}

		/// <summary></summary>
		public void Log( Exception exception )
		{
			this.Default.Log( exception );
		}

		/// <summary></summary>
		public void Log( Exception exp, params object[] args )
		{
			this.Default.Log( exp, args );
		}

		#endregion [ Methods ]
	}
}
