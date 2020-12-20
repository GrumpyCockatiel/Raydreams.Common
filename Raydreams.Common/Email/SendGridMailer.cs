using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Raydreams.Common.Email
{
	/// <summary>Concrete emailer for SendGrid</summary>
	public class SendGridMailer : IMailer
	{
		private string[] _to = { };
		private bool _html = false;
		private HttpStatusCode _last = HttpStatusCode.Unused;

		private string _key = String.Empty;

		/// <summary>Construct with the SendGrid API Key</summary>
        /// <param name="key">SendGrid API key</param>
		public SendGridMailer(string key)
		{
			this._key = key;
		}

		/// <summary></summary>
		public string[] To
		{
			get { return this._to; }
			set { this._to = value; }
		}

		/// <summary></summary>
		public bool IsHTML
		{
			get { return this._html; }
			set { this._html = value; }
		}

		/// <summary>The last HTTP response after a send email is complete</summary>
		public HttpStatusCode LastResponse
		{
			get { return this._last; }
			protected set { this._last = value; }
		}

		/// <summary>Actually send the email</summary>
		public async Task<bool> Send(string from, string subject, string body)
		{
			bool results = false;

			if ( String.IsNullOrWhiteSpace( _key ) )
				return false;

			if ( String.IsNullOrWhiteSpace( from ) )
				throw new System.ArgumentNullException(nameof(from), "From is required.");

			if ( String.IsNullOrWhiteSpace( subject ) )
				throw new System.ArgumentNullException( nameof( subject ), "Subject is required." );

			if (  String.IsNullOrWhiteSpace( body ) )
				throw new System.ArgumentNullException( nameof( body ), "Body is required." );

			try
			{
				SendGridClient mailer = new SendGridClient( this._key );
				SendGridMessage msg = new SendGridMessage();
				msg.SetFrom( new EmailAddress( from ) );
				msg.SetSubject( subject );
				msg.AddContent( (this._html) ? MimeType.Html : MimeType.Text, body );

				foreach ( string to in this.To )
					msg.AddTo( new EmailAddress( to ) );

				// wait for the reponse
				//Response response = mailer.SendEmailAsync( msg ).GetAwaiter().GetResult();
				Response response = await mailer.SendEmailAsync( msg );

				this.LastResponse = response.StatusCode;

				results = (response.StatusCode == HttpStatusCode.Accepted || response.StatusCode == HttpStatusCode.OK);
			}
			catch ( System.Exception)
			{
				return false;
			}

			return results;
		}
	}
}
