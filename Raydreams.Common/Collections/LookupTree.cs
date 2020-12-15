using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raydreams.Common.Collections
{
	/// <summary>A tree with a fast lookup List into each node of the tree.</summary>
	/// <typeparam name="T"></typeparam>
	/// <remarks>Since the lookup is a list and not a dictionary - the same item may be in there more than once.</remarks>
	public class LookupTree<T>
	{
		protected TreeNode<T> _root = null;

		public LookupTree(TreeNode<T> root)
		{
			this._root = root;
			this.Directory = new List<TreeNode<T>>();
		}

		/// <summary>A list of all the nodes themselves</summary>
		protected List<TreeNode<T>> Directory { get; set; }

		/// <summary>Get the root node as a regular tree</summary>
		public TreeNode<T> Root { get { return this._root; } }

		/// <summary>Get a list of all the data itself wihtout node info</summary>
		public List<T> Data
		{
			get
			{
				List<T> list = new List<T>();

				foreach ( TreeNode<T> node in this.Directory )
					list.Add(node.Data);
				return list;
			}
		}

		/// <summary>Add a new node to the tree</summary>
		/// <param name="parent"></param>
		/// <param name="child"></param>
		public void Add( TreeNode<T> parent, TreeNode<T> child)
		{
			// add the child to the parent
			parent.Add( child );

			// add the child to the list
			this.Directory.Add( child );
		}
	}
}
