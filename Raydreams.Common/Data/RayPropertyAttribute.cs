using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Raydreams.Common.Data
{
	/// <summary>Use to mark an object property with a source field to be read from.</summary>
	[AttributeUsage( AttributeTargets.Property, AllowMultiple = true )]
	public class RayPropertyAttribute : Attribute
	{
		#region [ Fields ]

		protected string _src = String.Empty;
		protected string _ctx = String.Empty;
		protected uint _order = 0;
		protected string _dest = String.Empty;

		#endregion [ Fields ]

		#region [ Constructors ]

		/// <summary>A source only for a specific context</summary>
		/// <param name="src"></param>
		/// <param name="context"></param>
		//public RayPropertyAttribute( string src, string context = null )
		//{
		//	this.Source = src;
		//	this.Context = context;
		//}

		#endregion [ Constructors ]

		#region [ Properties ]

		/// <summary>Applies to every context</summary>
		public bool EveryContext
		{
			get { return (String.IsNullOrWhiteSpace( this.Context )); }
		}

		/// <summary>The field this property maps from</summary>
		public string Source
		{
			get { return this._src; }
			set
			{
				if ( !String.IsNullOrWhiteSpace( value ) )
					this._src = value.Trim();
			}
		}

		/// <summary>The field this property maps to</summary>
		public string Destination
		{
			get { return this._dest; }
			set
			{
				if ( !String.IsNullOrWhiteSpace( value ) )
					this._dest = value.Trim();
			}
		}

		/// <summary></summary>
		public string Context
		{
			get { return this._ctx; }
			set
			{
				if ( !String.IsNullOrWhiteSpace( value ) )
					this._ctx = value.Trim();
			}
		}

		/// <summary>Used to specify in what order to write out the fields</summary>
		public uint Order
		{
			get { return this._order; }
		}

		#endregion [ Properties ]

		#region [ Methods ]

		/// <summary>Finds the first context declaration in a FieldSource</summary>
		/// <returns></returns>
		public static string GetDefaultContext( Type type )
		{
			PropertyInfo[] props = type.GetProperties( BindingFlags.Public | BindingFlags.Instance );

			if ( props.Length < 1 )
				return null;

			foreach ( PropertyInfo prop in props )
			{
				RayPropertyAttribute map = null;

				map = prop.GetCustomAttributes<RayPropertyAttribute>( false ).FirstOrDefault();

				if ( map != null && !String.IsNullOrWhiteSpace( map.Context ) )
					return map.Context;
			}

			return null;
		}

		/// <summary>Gets all the destination field names as a list sorted by their order</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static List<string> GetDestinations( Type type, string context = null )
		{
			List<Tuple<uint, string>> rec = new List<Tuple<uint, string>>();

			PropertyInfo[] props = type.GetProperties( BindingFlags.Public | BindingFlags.Instance );

			if ( props.Length < 1 )
				return new List<string>();

			foreach ( PropertyInfo prop in props )
			{
				RayPropertyAttribute map = null;

				if ( String.IsNullOrWhiteSpace( context ) )
				{
					map = prop.GetCustomAttributes<RayPropertyAttribute>( false ).FirstOrDefault();
					context = (map != null && !String.IsNullOrWhiteSpace( map.Context )) ? map.Context : null;
				}
				else
					map = prop.GetCustomAttributes<RayPropertyAttribute>( false ).Where( a => a.Context.Equals( context, StringComparison.Ordinal ) ).FirstOrDefault();

				if ( map == null )
					continue;

				if ( !String.IsNullOrWhiteSpace( map.Destination ) && prop.CanRead )
				{
					rec.Add( new Tuple<uint, string>( map.Order, map.Destination ) );
				}
			}

			// sort by order
			return rec.OrderBy( t => t.Item1 ).Select( t => t.Item2 ).ToList<string>();
		}

		#endregion [ Methods ]
	}
}
