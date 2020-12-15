using System;

namespace Raydreams.Common.Config
{
	/// <summary>A database configuration entity used by the DB Config Manager.</summary>
	public class DBConfigValue
	{
		public string Key { get; set; }
		public string Value { get; set; }
		public Type DataType { get; set; }
		public bool ReadOnly { get; set; }
		public bool IsNull { get; set; }
	}
}
