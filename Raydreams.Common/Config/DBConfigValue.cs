using System;

namespace Raydreams.Common.Config
{
	/// <summary>A database configuration entity used by the DB Config Manager.</summary>
	public class DBConfigValue
	{
		/// <summary></summary>
		public string Key { get; set; }

		/// <summary></summary>
		public string Value { get; set; }

		/// <summary></summary>
		public Type DataType { get; set; }

		/// <summary></summary>
		public bool ReadOnly { get; set; }

		/// <summary></summary>
		public bool IsNull { get; set; }
	}
}
