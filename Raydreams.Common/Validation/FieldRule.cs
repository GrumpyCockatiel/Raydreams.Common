using System;
using System.Reflection;

namespace Raydreams.Common.Validation
{
	/// <summary></summary>
	public class FieldRule
	{
		public FieldRule()
		{
		}

		/// <summary>The object property value to validate</summary>
		public PropertyInfo Property { get; set; }

		/// <summary>The validator to use</summary>
		public IFieldValidator<string> Validator { get; set; }

		protected void AddRule()
		{
		}
	}
}
