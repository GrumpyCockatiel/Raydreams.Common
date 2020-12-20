using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raydreams.Common.Email
{
	/// <summary>Generic email interface</summary>
	public interface IMailer
	{
		/// <summary>HTML format or not</summary>
		bool IsHTML { get; set; }

		/// <summary>List of valid email addresses to send to</summary>
		string[] To { get; set; }

		/// <summary>Actually send the message async</summary>
		Task<bool> Send( string from, string subject, string body );
	}
}
