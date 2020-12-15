//using System;
//using System.Net.Mail;
//using System.IO;
//using System.Threading.Tasks;

//namespace Raydreams.Common.Email
//{
//	/// <summary>Concrete emailer uses an SMTP Server</summary>
//	public class EmailSender : IMailer
//	{
//		#region [Fields]

//		private string _mailFrom = null;
//		private string _mailHost = null;
//		private bool _html = false;
//		private MailMessage _curMsg = null;

//		#endregion [Fields]

//		#region [Properties]

//		/// <summary></summary>
//		public string[] To { get; set; } = { };

//		/// <summary></summary>
//		public string MailHost
//		{
//			get { return this._mailHost; }
//			set { this._mailHost = value; }
//		}

//		/// <summary></summary>
//		public string MailFrom
//		{
//			get { return this._mailFrom; }
//			set { this._mailFrom = value; }
//		}

//		/// <summary></summary>
//		public bool HTMLFormat
//		{
//			get { return this._html; }
//			set { this._html = value; }
//		}

//		#endregion [Properties]

//		#region [Methods]

//		/// <summary>Creates the actual Mail message based on all the parameters</summary>
//		public bool PrepareMessage( string to, string subject, string body )
//		{
//			if ( String.IsNullOrEmpty(to) )
//				return false;

//			// set-up the message
//			this._curMsg = new MailMessage(this.MailFrom, to);

//			this._curMsg.Subject = subject;
//			this._curMsg.Body = body;
//			this._curMsg.IsBodyHtml = this.HTMLFormat;

//			return true;
//		}

//		///// <summary></summary>
//		//public bool AddAttachment( Stream data, string fileName )
//		//{
//		//	if ( this._curMsg == null )
//		//		return false;

//		//	// Create the file attachment for this e-mail message.
//		//	Attachment attachment = new Attachment(data, fileName);

//		//	this._curMsg.Attachments.Add(attachment);

//		//	return true;
//		//}

//		/// <summary></summary>
//		/// <param name="to"></param>
//		/// <param name="subject"></param>
//		/// <param name="body"></param>
//		/// <returns>Returns a 1 if the email was sent without error.</returns>
//		public async Task<bool> Send( string from, string subject, string body )
//		{
//			if (String.IsNullOrWhiteSpace(from))
//				throw new System.ArgumentNullException(nameof(from), "From is required.");

//			if (String.IsNullOrWhiteSpace(subject))
//				throw new System.ArgumentNullException(nameof(subject), "Subject is required.");

//			if (String.IsNullOrWhiteSpace(body))
//				throw new System.ArgumentNullException(nameof(body), "Body is required.");

//			// set-up the message
//			this._curMsg = new MailMessage(from, this.To);

//			this._curMsg.Subject = subject;
//			this._curMsg.Body = body;
//			this._curMsg.IsBodyHtml = this.HTMLFormat;

//			if ( this._curMsg == null )
//				return false;

//			// get the SMTP client
//			SmtpClient client = new SmtpClient(this.MailHost);

//			// send the message
//			await client.SendAsync(this._curMsg, null);

//			// clear the message
//			this.Clear();

//			// return success
//			return true;
//		}

//		/// <summary></summary>
//		public void Clear()
//		{
//			this._curMsg = null;
//		}

//		#endregion [Methods]
//	}
//}


// Email Sender interface
//public interface IEmailSender
//{
//	/// <summary>The SMTP mail server to use</summary>
//	string MailHost { get; set; }

//	/// <summary>The email from address to use</summary>
//	string MailFrom { get; set; }

//	/// <summary>Whether to send the email as HTML format or plain</summary>
//	bool HTMLFormat { get; set; }

//	/// <summary>Explicit preparation of the message before sending</summary>
//	bool PrepareMessage( string to, string subject, string body );

//	/// <summary>Add an attachemtn to the email.</summary>
//	bool AddAttachment( Stream data, string fileName );

//	/// <summary>Actually send the email.</summary>
//	bool Send();

//	/// <summary>Remove the contents of the message but keep all other settings.</summary>
//	void Clear();
//}