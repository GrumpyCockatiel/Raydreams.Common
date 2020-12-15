using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Raydreams.Common.Utilities
{
	/// <summary>Parses all the command line arguments into a dictionary.</summary>
	/// <example>
	/// Valid parameters forms:
	/// {-,/,--}param{ ,=,:}((",')value(",'))
	/// Examples: -param1 value1 --param2 /param3:"Test-:-work" /param4=happy -param5 '--=nice=--'
	/// </example>
	/// <remarks>
	/// From https://gist.github.com/shadowfox/5844284
	/// </remarks>
	public class CommandLineArguments : IEnumerable
	{
		#region [ Fields ]

		private Dictionary<string, string> _args;

		#endregion [ Fields ]

		// Constructor
		public CommandLineArguments(string[] clItems)
		{
			this._args = new Dictionary<string, string>();

			// regular expressions to split keys and values
			Regex spliter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			Regex remover = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

			string curParam = null;

			string[] parts;

			// iterate over the args list
			foreach (string str in clItems )
			{
				// Look for new parameters (-,/ or --) and a possible enclosed value (=,:)
				parts = spliter.Split(str, 3);
				
				switch (parts.Length)
				{
					// Found a value (for the last parameter found (space separator))
					case 1:

						if (curParam != null)
						{
							if ( !this._args.ContainsKey(curParam) )
							{
								parts[0] = remover.Replace(parts[0], "$1");
								this._args.Add( curParam, parts[0] );
							}
							curParam = null;
						}
						
						// else Error: no parameter waiting for a value (skipped)
						break;
					
					// Found just a parameter
					case 2:

						// The last parameter is still waiting. With no value, set it to true.
						if (curParam != null)
						{
							if ( !this._args.ContainsKey(curParam) )
								this._args.Add(curParam, "true");
						}
						curParam = parts[1];
						break;

					// Parameter with enclosed value
					case 3:

						// The last parameter is still waiting. With no value, set it to true.
						if (curParam != null)
						{
							if ( !this._args.ContainsKey(curParam) )
								this._args.Add(curParam, "true");
						}

						curParam = parts[1];

						// Remove possible enclosing characters (",')
						if (!this._args.ContainsKey(curParam))
						{
							parts[2] = remover.Replace(parts[2], "$1");
							this._args.Add(curParam, parts[2]);
						}

						curParam = null;
						break;
				}
			}

			// In case a parameter is still waiting
			if (curParam != null)
			{
				if ( !this._args.ContainsKey(curParam) )
					this._args.Add(curParam, "true");
			}
		}

		/// <summary></summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)this._args.GetEnumerator();
		}

		/// <summary>See if the key exists, was created</summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool Exists(string key)
		{
			if ( String.IsNullOrWhiteSpace(key) )
				return false;

			return this._args.ContainsKey(key);
		}

		/// <summary>Retrieve a parameter value if it exists</summary>
		/// <param name="Param"></param>
		/// <returns></returns>
		public string this[string Param]
		{
			get { return (this._args[Param]); }
		}

	}
}
