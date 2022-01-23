using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Raydreams.Common.Data
{
	/// <summary>Determines the type of context to use with RayProperty Attributes</summary>
	/// <remarks>
	/// Contenxt Precedence is determined by the RayProperty attribute on the Data Object class
	/// If there are no attributes on any property AND no context passed then a straight PropertyName mapping used
	/// If there are no attributes on any property BUT some context passed -> thats an error and return nothing
	/// If no context is passed (null context) but there are some attributes - then only consider those with an equal empty or null Context value aka Default Context
	/// If a context is passed then use only Properties in the matching context.
	/// </remarks>
	public enum RayContext
    {
		/// <summary>explicit context was passed but there are no adorned attributes -> return nothing</summary>
		Error = 0,
		/// <summary>no context or attributes passed -> Exact name match</summary>
		PropertyName = 1,
		/// <summary>no context passed, match on attributes with no context defined</summary>
		Null = 2,
		/// <summary>exact match on an attribute context value</summary>
		Explicit = 3
    }

	/// <summary>Use to mark an object property with data source/destination field metadata</summary>
	/// <remarks>Constructor can only be used to mark a source or destination field.
	/// Otherwise use named properties to use one attribute for both.
    /// To ignore some properties, adorn JUST the properties to insert or read
	/// Need to handle enum serializaton to strings
    /// </remarks>
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

		/// <summary>Empty Constructor</summary>
		public RayPropertyAttribute() { }

        /// <summary>A source only for a specific context</summary>
        /// <param name="src"></param>
        /// <param name="context"></param>
        public RayPropertyAttribute( string src, string context )
        {
            this.Source = src;
            this.Context = context;
        }

		/// <summary>A destination only for a specific context</summary>
		/// <param name="src"></param>
		/// <param name="context"></param>
		public RayPropertyAttribute( string dest, string context, uint order )
        {
			this.Destination = dest;
			this.Context = context;
			this.Order = order;
		}

		#endregion [ Constructors ]

		#region [ Properties ]

		/// <summary>Is this property null or empty context</summary>
		public bool IsNullContext => String.IsNullOrWhiteSpace( this.Context );

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

		/// <summary>The context this attribute applies to</summary>
		public string Context
		{
			get { return this._ctx; }
			set
			{
				if ( !String.IsNullOrWhiteSpace( value ) )
					this._ctx = value.Trim();
			}
		}

		/// <summary>Used to specify in what order to write out the fields where applicable</summary>
		public uint Order
		{
			get { return this._order; }
			set { this._order = value; }
		}

		#endregion [ Properties ]

		#region [ Methods ]

		/// <summary>Is this type adorned with any RayAttribute on any property</summary>
        /// <param name="type"></param>
        /// <returns></returns>
		public static bool Marked( Type type )
        {
			PropertyInfo[] props = type.GetProperties( BindingFlags.Public | BindingFlags.Instance );

			if ( props.Length < 1 )
				return false;

			foreach ( PropertyInfo prop in props )
			{
				RayPropertyAttribute map = prop.GetCustomAttributes<RayPropertyAttribute>( false ).FirstOrDefault();

				if ( map != null  )
					return true;
			}

			return false;
		}

		/// <summary>Calculates which type of conext should be used based on the object type and context name</summary>
		/// <returns></returns>
		public static RayContext CalculateContext( Type type, string context )
		{
			PropertyInfo[] props = type.GetProperties( BindingFlags.Public | BindingFlags.Instance );

			// no public instance properties
			if ( props.Length < 1 )
				return RayContext.Error;

			// named context is requested
			if ( !String.IsNullOrWhiteSpace( context ) )
			{
				// named context satisified
				if ( props.Select( p => p.GetCustomAttributes<RayPropertyAttribute>( true ).Where( a => a.Context.Equals( context.Trim(), StringComparison.Ordinal ) ).FirstOrDefault() ).Count() > 0 )
					return RayContext.Explicit;
				else // explicit context but no matching attributes
					return RayContext.Error;
			}

			// now check for null context mean there are adorned properties with no explicit context
			if ( props.Select( p => p.GetCustomAttributes<RayPropertyAttribute>( true ).Where( a => a.IsNullContext ).FirstOrDefault() ).Count() > 0 )
				return RayContext.Null;

			// match only by explicit property name
			return RayContext.PropertyName;
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

				// null context
				if ( String.IsNullOrWhiteSpace( context ) )
				{
					map = prop.GetCustomAttributes<RayPropertyAttribute>( false ).FirstOrDefault();
					context = (map != null && !String.IsNullOrWhiteSpace( map.Context )) ? map.Context : null;
				}
				else // explicit context
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
