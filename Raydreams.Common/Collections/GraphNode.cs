using System;
using System.Collections.Generic;
using System.Text;

namespace Raydreams.Common.Collections
{
	/// <summary>The GraphNode inhierits from a generic node to create graph structures.</summary>
	/// <typeparam name="T">The data type of the data the node stores.</typeparam>
	public class GraphNode<T> : Node<T>, IEnumerable<GraphNode<T>>
	{
		#region [Fields]

		/// <summary>The list of nodes that are connected to this node.</summary>
		private List<GraphNode<T>> _neighbors = null;

		#endregion [Fields]

		#region [Properties]

		/// <summary>Returns the number of neighbors this node has.</summary>
		public int Count
		{
			get
			{
				if ( this._neighbors == null )
					return 0;

				return this._neighbors.Count;
			}
		}

		#endregion [Properties]

		#region [Methods]

		/// <summary>Add a neighbor node to this node.</summary>
		/// <returns>The total number of neighbors to this node after the addition.</returns>
		public int Add( GraphNode<T> neighbor )
		{
			if ( neighbor == null )
				return 0;

			// create the neighbor list if it does not exist
			if ( this._neighbors == null )
				this._neighbors = new List<GraphNode<T>>( 1 );

			// only add if not already a neighbor
			if ( !this._neighbors.Contains( neighbor ) )
			{
				// add to this node's neighbor's
				this._neighbors.Add( neighbor );

				// add this node as a neighbor to the other node
				neighbor.Add( this );
			}

			// return the total number of neighbors
			return this._neighbors.Count;
		}

		/// <summary>Adds a collection of neighbor elements to this node.</summary>
		/// <param name="collection">The collection of neighbors to add.</param>
		public void AddRange( IEnumerable<GraphNode<T>> collection )
		{
			foreach ( GraphNode<T> n in collection )
				this.Add( n );
		}

		/// <summary>Return whether this node even has neighbors or not.</summary>
		public virtual bool HasNeighbors()
		{
			if ( this._neighbors == null )
				return false;

			return ( this._neighbors.Count > 0 ) ? false : true;
		}

		/// <summary>Set this node to a state of having no neighbors.</summary>
		public void Clear()
		{
			if ( this._neighbors != null )
				this._neighbors.Clear();
		}

		/// <summary>Returns an enumerator to iterate this node's neighbors.</summary>
		public IEnumerator<GraphNode<T>> GetEnumerator()
		{
			return this._neighbors.GetEnumerator();
		}

		/// <summary>Returns an enumerator to iterate this node's neighbors.</summary>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion [Methods]

	}
}
