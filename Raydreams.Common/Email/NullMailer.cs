using System;
using System.Text;
using System.Threading.Tasks;

namespace Raydreams.Common.Email
{
	/// <summary></summary>
	public class NullMailer : IMailer
	{
		#region [Properties]

		/// <summary></summary>
		public string[] To { get; set; } = { };

		/// <summary></summary>
		public bool IsHTML { get; set; } = false;

		/// <summary>Reply to email address</summary>
		public string ReplyTo { get; set; } = null;

		#endregion [Properties]

		/// <summary></summary>
		public Task<bool> Send( string from, string subject, string body )
		{
			Task<bool> task = new Task<bool>(() => {
				return true;
			});
			task.Start();

			return task;
		}
	}
}
