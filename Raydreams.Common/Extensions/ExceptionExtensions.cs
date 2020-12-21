using System.Text;

namespace Raydreams.Common.Extensions
{
	/// <summary>Extensions to System.Exception</summary>
	public static class ExceptionExtensions
	{
		/// <summary>Creates a verbose string message from an exception</summary>
		/// <param name="exp">The exception itself</param>
		/// <param name="includeStackTrace">True if you want the full stack trace, otherwise defaults to false</param>
		/// <returns></returns>
		public static string ToLogMsg(this System.Exception exp, bool includeStackTrace = false)
		{
			StringBuilder msg = new StringBuilder( $"{exp.GetType().FullName} : {exp.Message} " );

			if ( includeStackTrace )
				msg.Append( exp.StackTrace );

			// get the inner exception if there is one
			if ( exp.InnerException != null )
			{
				msg.AppendFormat( "{0}; {1} ", msg, exp.InnerException.Message );
				if ( includeStackTrace )
					msg.Append( exp.InnerException.StackTrace );
			}

			return msg.ToString();
		}
	}
}
