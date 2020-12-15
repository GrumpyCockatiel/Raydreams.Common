using Raydreams.Common.Email;
using Raydreams.Common.Extensions;
using Raydreams.Common.Model;
using System;
using System.Text;

namespace Raydreams.Common.Logging
{
	/// <summary>Sends logs via email by holding logs in a string until send is called</summary>
	public class EmailLogger : ILogger
	{
		#region [ Fields ]

		private IMailer _mailer = null;
		private LogLevel _level = LogLevel.Off;
		private string _mailto = null;
		private string _src = null;
		private string _subject = null;
		private StringBuilder _body = null;
		private bool _send = true;

		#endregion [ Fields ]

		#region [ Constructors ]

		/// <summary></summary>
		/// <param name="source">Application sending the log</param>
		/// <param name="to">The address(s) to send to</param>
		/// <param name="mailer">Needs an EmailSender to send the email</param>
		/// <param name="baseLevel">Minimum level to log.</param>
		public EmailLogger( string source, string to, IMailer mailer, LogLevel baseLevel, bool enable = true )
		{
			this.Mailer = mailer;
            this.MailTo = to;
            this.Level = baseLevel;
			this.Source = source;
			this.Enabled = enable;
			this._body = new StringBuilder();
		}

		#endregion [ Constructors ]

		#region [ Properties ]

		/// <summary>The minimum level inclusive to log based on the LogLevel enumeration [level,...]</summary>
		public LogLevel Level
		{
			get { return this._level; }
			set { this._level = value; }
		}

		/// <summary>The mailer to use to actually send the email log.</summary>
		public IMailer Mailer
		{
			get { return this._mailer; }
			set { this._mailer = value; }
		}

		/// <summary>Enable or disable to toggle sending emails</summary>
		/// <value>Set to true to send out emails, otherwise false will turn it off.</value>
		public bool Enabled
		{
			protected get { return this._send; }
			set { this._send = value; }
		}

		/// <summary></summary>
		public string MailTo
		{
			get { return this._mailto; }
			set
			{
				if ( !String.IsNullOrWhiteSpace( value ) )
					this._mailto = value.Trim();
			}
		}

		/// <summary>The current email body text</summary>
		private StringBuilder Body
		{
			get { return this._body; }
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

		/// <summary>The logging source. Who is doing the logging.</summary>
		public string Subject
		{
			get
			{
				if ( String.IsNullOrWhiteSpace( this._subject ) )
					return String.Format( "Log for {0}", this.Source );

				return this._subject;
			}
			set { this._subject = value; }
		}

		#endregion [ Properties ]

		#region [ Methods ]

		/// <summary>Send the email log</summary>
		public void Send()
		{
			// if emailing is disabled, then dont bother
			if ( !this.Enabled )
				return;

			if ( this.Mailer == null || String.IsNullOrWhiteSpace(this.MailTo) )
				return;

			this.Mailer.To = new string[] { this.MailTo };
			this.Mailer.Send( "noreply@raydreams.com" , this.Subject, this.Body.ToString() );
		}

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

			if ( lvl < this.Level )
				return 0;

			// append level
			this.Body.AppendFormat( "{0}|{1}|", DateTime.UtcNow.ToString( "s" ), lvl );

			// append category
			if ( String.IsNullOrWhiteSpace( category ) )
				this.Body.Append( "<none>|" );
			else
				this.Body.AppendFormat( "{0}|", category );

			if ( !String.IsNullOrWhiteSpace( msg ) )
				this.Body.AppendFormat( "{0}", msg.Trim() );

			// convert the args dictionary to a string and add to the end
			if ( args != null && args.Length > 0 )
				this.Body.AppendFormat( "|args={0}", String.Join( ";", args ) );

			// outlook needs a spaces at the end to honor newlines?
			// http://stackoverflow.com/questions/136052/how-do-i-format-a-string-in-an-email-so-outlook-will-print-the-line-breaks
			this.Body.Append( "   " );
			this.Body.AppendLine();

			return 1;
		}

		#endregion [ Methods ]
	}
}
