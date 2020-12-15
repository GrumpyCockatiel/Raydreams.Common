using System;
using System.Collections.Generic;

namespace Raydreams.Common.Collections
{
	/// <summary>Generic binary tree structure</summary>
	public class BinaryTreeNode<T> : Node<T>, IEnumerable<BinaryTreeNode<T>>
	{
		#region [Fields]

		/// <summary>Left child node.</summary>
		private BinaryTreeNode<T> _left = null;
		/// <summary>Right child node.</summary>
		private BinaryTreeNode<T> _right = null;
		/// <summary>Reference to this node's parent node.</summary>
		private BinaryTreeNode<T> _parent = null;

		#endregion [Fields]

		#region [Properties]

		/// <summary>Gets or sets this tree node's left child.</summary>
		public BinaryTreeNode<T> Left
		{
			get { return this._left; }
			set { this._left = value; }
		}

		/// <summary>Gets or sets this tree node's right child.</summary>
		public BinaryTreeNode<T> Right
		{
			get { return this._right; }
			set { this._right = value; }
		}

		/// <summary>Gets or sets this tree node's parent node.</summary>
		public BinaryTreeNode<T> Parent
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
				BinaryTreeNode<T> current = this._parent;

				while ( current != null )
				{
					current = current.Parent;
					++depth;
				}

				return depth;
			}
		}

		/// <summary>Gets the tree node that is the root of the subtree this node is contained within.</summary>
		public BinaryTreeNode<T> Root
		{
			get
			{
				BinaryTreeNode<T> current = this;

				while ( current.Parent != null )
					current = current.Parent;

				return current;
			}
		}

		#endregion [Properties]

		#region [Methods]

		/// <summary>Return whether or not this node has child nodes.</summary>
		public bool HasChildren()
		{
			return ( this._left == null && this._right == null ) ? false : true;
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

		/// <summary>Set this node to a state of having no children.</summary>
		public void Clear()
		{
			this._left = null;
			this._right = null;
		}

		/// <summary></summary>
		public IEnumerator<BinaryTreeNode<T>> GetEnumerator()
		{
			return this.GetEnumerator( TraversalMethod.Preorder );
		}

		/// <summary></summary>
		public IEnumerator<BinaryTreeNode<T>> GetEnumerator( TraversalMethod method )
		{
			switch ( method )
			{
				case TraversalMethod.Postorder:
					return Postorder.GetEnumerator();

				case TraversalMethod.Inorder:
					return Inorder.GetEnumerator();

				case TraversalMethod.Levelorder:
					return Levelorder.GetEnumerator();

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
		public IEnumerable<BinaryTreeNode<T>> Preorder
		{
			get
			{
				// create a state stack
				Stack<BinaryTreeNode<T>> toVisit = new Stack<BinaryTreeNode<T>>();
				BinaryTreeNode<T> current = null;

				// init the stack with the root
				toVisit.Push( this );

				while ( toVisit.Count > 0 )
				{
					// make the top of the stack the current node
					current = toVisit.Pop();

					// push any children
					if ( current._left != null )
						toVisit.Push( current._left );

					if ( current._right != null )
						toVisit.Push( current._right );

					// return the current node
					yield return current;
				}
			}
		}

		/// <summary>Creates a inorder enumerator.</summary>
		public IEnumerable<BinaryTreeNode<T>> Inorder
		{
			get
			{
				throw new System.Exception( "Not implemented." );
			}
		}

		/// <summary>Creates a postorder enumerator.</summary>
		public IEnumerable<BinaryTreeNode<T>> Postorder
		{
			get
			{
				throw new System.Exception( "Not implemented." );
			}
		}

		/// <summary>Creates a level-order enumerator.</summary>
		public IEnumerable<BinaryTreeNode<T>> Levelorder
		{
			get
			{
				throw new System.Exception( "Not implemented." );
			}
		}

		#endregion [Traversal Methods]
	}
}
