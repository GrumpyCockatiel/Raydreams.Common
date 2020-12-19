using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Raydreams.Common.Data
{
    //public class RayDataFieldAttribute
    //{
    //    #region [ Fields ]

    //    protected string _ctx = String.Empty;
    //    protected uint _order = 0;
    //    protected string _dest = String.Empty;

    //    #endregion [ Fields ]

    //    #region [ Constructors ]

    //    /// <summary>Given a source field only, defaults to no destination and order 0</summary>
    //    /// <param name="fieldName">The destination field for this property.</param>
    //    /// <param name="context">A context if more than one destination is specified</param>
    //    /// <param name="order">The export order of this field</param>
    //    public RayDataFieldAttribute(string fieldName, string context = null, uint order = 0)
    //    {
    //        this.FieldName = fieldName;
    //        this.Context = context;
    //        this._order = order;
    //    }

    //    #endregion [ Constructors ]

    //    #region [ Properties ]

    //    /// <summary>The field this property maps to</summary>
    //    public string FieldName
    //    {
    //        get { return this._dest; }
    //        set
    //        {
    //            if (!String.IsNullOrWhiteSpace(value))
    //                this._dest = value.Trim();
    //        }
    //    }

    //    /// <summary></summary>
    //    public string Context
    //    {
    //        get { return this._ctx; }
    //        set
    //        {
    //            if (!String.IsNullOrWhiteSpace(value))
    //                this._ctx = value.Trim();
    //        }
    //    }

    //    /// <summary>Used to specify in what order to write out the fields</summary>
    //    public uint Order
    //    {
    //        get { return this._order; }
    //    }

    //    #endregion [ Properties ]

    //    #region [ Methods ]

    //    /// <summary>Gets all the destination field names as a list sorted by their order</summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <returns></returns>
    //    public static List<string> GetDestinations(Type type, string context = null)
    //    {
    //        List<Tuple<uint, string>> rec = new List<Tuple<uint, string>>();

    //        PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

    //        if (props.Length < 1)
    //            return new List<string>();

    //        foreach (PropertyInfo prop in props)
    //        {
    //            FieldDestinationAttribute map = null;

    //            if (String.IsNullOrWhiteSpace(context))
    //            {
    //                map = prop.GetCustomAttributes<FieldDestinationAttribute>(false).FirstOrDefault();
    //                context = (map != null && !String.IsNullOrWhiteSpace(map.Context)) ? map.Context : null;
    //            }
    //            else
    //                map = prop.GetCustomAttributes<FieldDestinationAttribute>(false).Where(a => a.Context.Equals(context, StringComparison.Ordinal)).FirstOrDefault();

    //            if (map == null)
    //                continue;

    //            if (!String.IsNullOrWhiteSpace(map.Destination) && prop.CanRead)
    //            {
    //                rec.Add(new Tuple<uint, string>(map.Order, map.Destination));
    //            }
    //        }

    //        // sort by order
    //        return rec.OrderBy(t => t.Item1).Select(t => t.Item2).ToList<string>();
    //    }

    //    #endregion [ Methods ]
    //}
}
