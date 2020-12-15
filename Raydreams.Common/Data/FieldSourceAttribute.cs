using System;
using System.Linq;
using System.Reflection;

namespace Raydreams.Common.Data
{
	/// <summary>Use to mark an object property with a source field to be read from..</summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class FieldSourceAttribute : Attribute
	{
		#region [ Fields ]

		protected string _src = String.Empty;
		protected string _ctx = String.Empty;

		#endregion [ Fields ]

		#region [ Constructors ]

		/// <summary>A source only for a specific context</summary>
		/// <param name="src"></param>
		/// <param name="context"></param>
		public FieldSourceAttribute(string src, string context = null)
		{
			this.Source = src;
			this.Context = context;
		}

		#endregion [ Constructors ]

		#region [ Properties ]

		/// <summary>Applies to every context</summary>
		public bool EveryContext
		{
			get { return (String.IsNullOrWhiteSpace(this.Context)); }
		}

		/// <summary>The field this property maps from</summary>
		public string Source
		{
			get { return this._src; }
			set
			{
				if (!String.IsNullOrWhiteSpace(value))
					this._src = value.Trim();
			}
		}

		/// <summary></summary>
		public string Context
		{
			get { return this._ctx; }
			set
			{
				if (!String.IsNullOrWhiteSpace(value))
					this._ctx = value.Trim();
			}
		}

		#endregion [ Properties ]

		#region [ Methods ]

		/// <summary>Finds the first context declaration in a FieldSource</summary>
		/// <returns></returns>
		public static string GetDefaultContext(Type type)
		{
			PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			if (props.Length < 1)
				return null;

			foreach (PropertyInfo prop in props)
			{
				FieldSourceAttribute map = null;

				map = prop.GetCustomAttributes<FieldSourceAttribute>(false).FirstOrDefault();

				if (map != null && !String.IsNullOrWhiteSpace(map.Context))
					return map.Context;
			}

			return null;
		}

		#endregion [ Methods ]
	}
}
