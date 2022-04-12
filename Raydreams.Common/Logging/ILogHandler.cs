using System;
using Raydreams.Common.Model;

namespace Raydreams.Common.Logging
{
	/// <summary>Simple delegate for sending a string message.</summary>
	/// <param name="msg">The message to send</param>
	public delegate void Echo( string msg );

	// <summary>A delegate to send a log message with more control over all the log properties than LogInfo</summary>
	public delegate void LogMessage( object sender, LogRecord message );

	// <summary>A delegate to send an exception</summary>
	public delegate void LogException( object sender, Exception exception, params object[] args );

	// <summary></summary>
	public interface ILogHandler
	{
		Echo Echo { get; set; }

		event LogException ExceptionHandler;

		event LogMessage MessageHandler;
	}

	/// <summary>A simple implementation of the LogHander</summary>
	public class BasicLogHandler : ILogHandler
	{
		public event LogException ExceptionHandler;

		public event LogMessage MessageHandler;

		/// <summary>A callback for echoing messages, usually to the console.</summary>
		public Echo Echo { get; set; }

		/// <summary>Log a message using defaults</summary>
		/// <param name="msg"></param>
		public virtual void OnLog( string msg )
		{
			if ( this.MessageHandler != null )
			{
				this.MessageHandler( this, new LogRecord() { Message = msg } );
			}
		}

		/// <summary>Log an exception</summary>
		/// <param name="e"></param>
		public virtual void OnLog( System.Exception e )
		{
			if ( this.ExceptionHandler != null )
			{
				this.ExceptionHandler( this, e );
			}
		}
	}

}