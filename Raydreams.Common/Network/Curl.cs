using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Raydreams.Common.Network
{
	/// <summary>Wraps a web request object.</summary>
	public class Curl
	{
		#region [Fields]
		private string _url = null;
		private string _results = null;
		private int _timeout = 10000;
		private string _contentType = null;
		private string _charset = null;
		private bool _removeNewlines = true;
		private bool _removeHtml = false;
		private string _userAgent = @"Mozilla/4.0 (compatible; MSIE+6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
		//private string			_userAgent = @"Mozilla/5.0 (Macintosh; U; PPC Mac OS X; en) AppleWebKit/416.11 (KHTML, like Gecko) Safari/416.12";
		#endregion [Fields]

		#region [Constructors]

		public Curl( string url )
		{
			this.Url = url;
		}

		#endregion [Constructors]

		#region [Properties]

		/// <summary></summary>
		public string ContentType
		{
			get { return this._contentType; }
		}

		/// <summary></summary>
		public string CharSet
		{
			get { return this._charset; }
		}

		/// <summary></summary>
		public string Url
		{
			get { return this._url; }
			set { this._url = value.Trim(); }
		}

		/// <summary>Get the text results of the request.</summary>
		public string Results
		{
			get { return this._results; }
		}

		/// <summary>Get or set the user agent to use in the request.</summary>
		public string UserAgent
		{
			get { return this._userAgent; }
			set { this._userAgent = value; }
		}

		/// <summary>Get or set whether to strip newlines from the results.</summary>
		public bool RemoveNewlines
		{
			get { return this._removeNewlines; }
			set { this._removeNewlines = value; }
		}

		/// <summary>Get or set whether to strip all HTML tags from the results.</summary>
		public bool RemoveHtml
		{
			get { return this._removeHtml; }
			set { this._removeHtml = value; }
		}

		/// <summary></summary>
		public int Timeout
		{
			get { return this._timeout; }
			set { if ( value > 0 ) this._timeout = value; }
		}

		#endregion [Properties]

		#region [Methods]

		/// <summary>Retrieves a web page.</summary>
		/// <param name="url"></param>
		public string Scrape()
		{
			// retrieve the resource
			this.GetResponse( this.Url );

			// post process as requested
			if ( this.RemoveNewlines )
				this._results = this._results.Replace( "\n", String.Empty );

			if ( this.RemoveHtml )
				this._results = new Regex( "<[^>]*>" ).Replace( this._results, String.Empty );

			return this._results;
		}

		/// <summary></summary>
		private void GetResponse( string url )
		{
			// create request
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create( url );
			req.Credentials = CredentialCache.DefaultCredentials;
			req.Timeout = this.Timeout;
			req.UserAgent = this.UserAgent;

			// get response
			try
			{
				HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
				Stream receiveStream = resp.GetResponseStream();

				this._charset = resp.CharacterSet;
				this._contentType = resp.ContentType;

				// read the response into a string			
				StreamReader readStream = new StreamReader( receiveStream, Encoding.GetEncoding( resp.CharacterSet ) );
				this._results = readStream.ReadToEnd();

				// close the stream
				readStream.Close();
				resp.Close();
			}
			catch ( System.Exception )
			{
				this._results = null;
			}
		}

		#endregion [Methods]
	}
}
