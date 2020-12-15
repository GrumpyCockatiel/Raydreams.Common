using System;
using System.Collections.Generic;

namespace Raydreams.Common.Collections
{
    /// <summary>The Node&lt;T&gt; class represents the base concept of a Node for a tree or graph.  It contains a data item of type T, and a list of neighbors.</summary>
    /// <typeparam name="T">The type of data contained in the Node.</typeparam>
	[Serializable()]
    public abstract class Node<T>
	{
		#region [Fields]

		/// <summary>The generic data contained in the node.</summary>
        private T _data = default(T);

		#endregion [Fields]

		#region [Constructors]

		/// <summary>Constructor that inserts no data.</summary>
        protected Node() : this(default(T))
		{}

		/// <summary>Create a new node with the input data but no neighbors.</summary>
		protected Node(T data)
		{
			this._data = data;
		}

		#endregion [Constructors]

		#region [Properties]

		/// <summary>Gets or sets the data in the node.</summary>
		public T Data
		{
			get { return this._data; }
			set { this._data = value; }
		}

		#endregion [Properties]

	}
}
