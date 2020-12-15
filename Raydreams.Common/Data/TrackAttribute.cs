using System;

namespace Raydreams.Common.Data
{
	/// <summary>An attribute to define if changes to this property are tracked or cared about. True mens we care about changes to the property.</summary>
	public class TrackAttribute : Attribute
	{
		public TrackAttribute( bool change )
		{
			this.TrackChange = change;
		}

		/// <summary></summary>
		public bool TrackChange { get; private set; }
	}
}
