using System;
using System.Collections.Generic;

namespace Raydreams.Common.Collections
{
	/// <summary>Provides the set of values by which a binary search tree can be enumerated.</summary>
	public enum TraversalMethod
	{
		/// <summary>Use a preorder (NLR), tree traversal method.</summary>
		Preorder = 0,
		/// <summary>Use a inorder (LNR), tree traversal method.</summary>
		Inorder,
		/// <summary>Use a postorder (LRN) (depth-first), tree traversal method.</summary>
		Postorder,
		/// <summary>Use a level-order (breadth-first), tree traversal method.</summary>
		Levelorder,
		/// <summary>Only iterate the node's immediate children.</summary>
		Children
	}

	/// <summary>The TreeNode inhierits from a generic node to create Mulitway (or N-way) tree structures.</summary>
	/// <typeparam name="T">The data type of the data the node stores.</typeparam>
	public class TreeNode<T> : Node<T>, IEnumerable<TreeNode<T>>
	{
		#region [Fields]

		/// <summary>The list of nodes that are connected to this node.</summary>
		private List<TreeNode<T>> _children = null;
		/// <summary>Reference to this node's parent node.</summary>
		private TreeNode<T> _parent = null;

		#endregion [Fields]

		#region [Properties]

		/// <summary>Gets or sets this tree node's parent node.</summary>
		public TreeNode<T> Parent
		{
			get { return this._parent; }
			set { this._parent = value; }
		}

		/// <summary>Gets the depth of this element.</summary>
		public int Depth
		{
			get
			{
				int depth = 0;
				TreeNode<T> current = this._parent;
				
				while ( current != null )
				{
					current = current.Parent;
					++depth;
				}

				return depth;
			}
		}

		/// <summary>Gets the tree node that is the root of the subtree this node is contained within.</summary>
		public TreeNode<T> Root
		{
			get
			{
				TreeNode<T> current = this;
				
				while ( current.Parent != null )
					current = current.Parent;

				return current;
			}
		}

		/// <summary>Return JUST the DIRECT descendents of this specific mode has</summary>
		public List<TreeNode<T>> DirectDescendants
		{
			get
			{
				if ( this.HasChildren() )
					return this._children;

				return null;
			}
		}

		//Path - returns a list from this node back up to the root.

		// Children - returns the enumerator to the children list, not the list itself.

		#endregion [Properties]

		#region [Methods]

		/// <summary>Add a child node to this node.</summary>
		/// <returns>The total number of children to this node after the addition.</returns>
		public int Add( TreeNode<T> child )
		{
			if ( child == null )
				return 0;

			// create the neighbor list if it does not exist
			if ( this._children == null )
				this._children = new List<TreeNode<T>>();

			// add to this node's children
			this._children.Add( child );

			// create a reference from the child to the parent
			child.Parent = this;

			// return the total number of children
			return this._children.Count;
		}

		/// <summary>Adds a collection of child elements to this node.</summary>
		/// <param name="collection">The collection of children to add.</param>
		public void AddRange( IEnumerable<TreeNode<T>> collection )
		{
			foreach ( TreeNode<T> n in collection )
				this.Add( n );
		}

		/// <summary>Return whether or not this node has child nodes.</summary>
		public bool HasChildren()
		{
			if ( this._children == null )
				return false;

			return ( this._children.Count > 0 ) ? true : false;
		}

		/// <summary>Returns whether this entity is the root entity or not.</summary>
		public bool IsRoot()
		{
			return ( this._parent == null );
		}

		/// <summary>Returns whether this node is a leaf element or not.</summary>
		public virtual bool IsLeaf()
		{
			return !this.HasChildren();
		}

		/// <summary>Find the node containing matching data.</summary>
		public TreeNode<T> Find( T data )
		{
			// iterate the tree doing a comparison on each node returned.
			throw new System.Exception( "Not implemented." );
		}

		/// <summary>Set this node to a state of having no children.</summary>
		public void Clear()
		{
			if ( this._children != null )
				this._children.Clear();
		}

		/// <summary></summary>
		public IEnumerator<TreeNode<T>> GetEnumerator()
		{
			return this.GetEnumerator( TraversalMethod.Preorder );
		}

		/// <summary></summary>
		public IEnumerator<TreeNode<T>> GetEnumerator( TraversalMethod method )
		{
			switch ( method )
			{
				case TraversalMethod.Postorder:
					return Postorder.GetEnumerator();

				case TraversalMethod.Inorder:
					return Inorder.GetEnumerator();

				case TraversalMethod.Levelorder:
					return Levelorder.GetEnumerator();

				case TraversalMethod.Children:
					return this._children.GetEnumerator();

				case TraversalMethod.Preorder:
				default:
					return Preorder.GetEnumerator();
			}
		}

		/// <summary></summary>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion [Methods]

		#region [Traversal Methods]

		/// <summary>Creates a preorder enumerator.</summary>
		public IEnumerable<TreeNode<T>> Preorder
		{
			get
			{
				// create a state stack
				Stack<TreeNode<T>> toVisit = new Stack<TreeNode<T>>();
				TreeNode<T> current = null;

				// init the stack with the root
				toVisit.Push( this );

				while ( toVisit.Count > 0 )
				{
					// make the top of the stack the current node
					current = toVisit.Pop();

					// push any children
					if ( current._children != null )
					{
						// iterate backward through the children
						for (int i = current._children.Count - 1; i > -1; --i)
						{
							// push the children onto the stack left to right
							toVisit.Push( current._children[i] );
						}
					}

					// return the current node
					yield return current;
				}
			}
		}

		/// <summary>Creates a level-order enumerator.</summary>
		public IEnumerable<TreeNode<T>> Levelorder
		{
			get
			{
				Queue<TreeNode<T>> toVisit = new Queue<TreeNode<T>>();
				Queue<TreeNode<T>> temp = new Queue<TreeNode<T>>();

				// init the stack with the root
				toVisit.Enqueue( this );

				while ( toVisit.Count > 0 || temp.Count > 0 )
				{
					while ( toVisit.Count > 0 )
					{
						// make the top of the stack the current node
						TreeNode<T> current = toVisit.Dequeue();

						if ( current.HasChildren() )
						{
							foreach ( TreeNode<T> c in current._children )
								temp.Enqueue( c );
						}

						// return the current node
						yield return current;
					}

					while ( temp.Count > 0 )
					{
						TreeNode<T> child = temp.Dequeue();

						toVisit.Enqueue( child );
					}

				}
			}
		}

		/// <summary>Creates a inorder enumerator.</summary>
		public IEnumerable<TreeNode<T>> Inorder
		{
			get
			{
				throw new System.Exception("Multi-way trees do not have inorder traversal.");
			}
		}

		/// <summary>Creates a postorder enumerator.</summary>
		/// <remarks>Starts with the bottom most children and works its way up</remarks>
		public IEnumerable<TreeNode<T>> Postorder
		{
			get
			{
				throw new System.Exception( "Not implemented." );
			}
		}

		#endregion [Traversal Methods]
	}
}
