using System.ComponentModel;

namespace Raydreams.Common.Model
{
	/// <summary>Enumerates the types of physical environments</summary>
	public enum EnvironmentType
	{
		/// <summary></summary>
		[Description("unk")]
		Unknown = 0,

		/// <summary></summary>
		[Description( "local" )]
		Local = 1,

		/// <summary></summary>
		[Description("dev")]
		Development = 2,

		/// <summary></summary>
		[Description("test")]
		Testing = 3,

		/// <summary></summary>
		[Description("stg")]
		Staging = 4,

		/// <summary></summary>
		[Description("train")]
		Training = 5,

		/// <summary></summary>
		[Description("prod")]
		Production = 10
	}
}
