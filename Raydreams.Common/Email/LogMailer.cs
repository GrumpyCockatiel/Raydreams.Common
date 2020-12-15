using System;
using System.Text;
using System.Threading.Tasks;
using Raydreams.Common.Logging;

namespace Raydreams.Common.Email
{
	/// <summary></summary>
	public class LogMailer : IMailer
	{
		#region [Fields]

		public static readonly string DefaultSubject = @"Default Subject";
		public static readonly string DefaultBody = $"This is a test sent on {DateTime.UtcNow}";
		private ILogger _logger = null;

		#endregion [Fields]

		public LogMailer(ILogger logger)
		{
			this._logger = logger;
		}

		#region [Properties]

		/// <summary></summary>
		public string[] To { get; set; } = { };

		/// <summary></summary>
		public bool IsHTML { get; set; } = false;

		/// <summary></summary>
		protected ILogger Logger
		{
			get
			{
				return this._logger ?? new NullLogger();
			}
		}

		#endregion [Properties]

		/// <summary></summary>
		public Task<bool> Send( string from, string subject, string body )
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat( "Subject={0};", subject );
			sb.AppendFormat( "From={0};", from );
			sb.AppendFormat( "To={0};", String.Join( ",", this.To ) );
			sb.AppendFormat( "Body={0}", body );

			Task<bool> task = new Task<bool>(() => {
				this.Logger.Log( sb.ToString(), "DEV", LogLevel.Info );
				return true;
			});
			task.Start();

			return task;
		}
	}
}
