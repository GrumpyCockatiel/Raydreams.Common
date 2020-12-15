using System;
using System.Collections;

namespace Raydreams.Common.Collections
{
	/// <summary>CircularList structure which when reaches the end starts over again automatically</summary>
	/// <remarks>Make to use index positions.</remarks>
	public class CircularList : IEnumerable
	{
		/// <summary></summary>
		private class ListItem
		{
			/// <summary>This item.</summary>
			public object Item;
			/// <summary>The item proceeding this item in the list.</summary>
			public ListItem Next;
			/// <summary>The item preceeding this item in the list.</summary>
			public ListItem Previous;

			/// <summary></summary>
			public ListItem( object x, ListItem n )
			{
				this.Item = x;
				this.Next = n;
			}

			public ListItem( object x, ListItem n, ListItem p )
				: this( x, n )
			{
				this.Previous = p;
			}
		}

		/// <summary></summary>
		private class CircularListEnumerator : IEnumerator
		{
			/// <summary>Reference to the list being iterated through.</summary>
			private CircularList list;
			/// <summary>Temporary counter that keeps track of how many items have been passed.</summary>
			private int idx = 0;

			/// <summary>Creates a new instance of a circular list enumerator.</summary>
			public CircularListEnumerator( CircularList l )
			{
				// create an internal reference to the list to operate on
				this.list = l;

				// init the counter
				this.idx = -1;

				// back the marker up one item
				//this.list.marker = this.list.marker.Previous;
			}

			/// <summary>Advance the marker one item and returns a reference to the new item.</summary>
			/// <remarks>Foreach calls MoveNext first.  If MoveNext returns true, foreach then calls Current.</remarks>
			public bool MoveNext()
			{
				// don't do anything if the list is empty
				if ( this.list.IsEmpty() )
					return false;

				// always advance the marker to loop the list
				this.list.marker = this.list.marker.Next;
				++this.idx;

				// if the current marker is already at the end of the list
				return ( this.idx < this.list.Length );
			}

			/// <summary>Resets the current marker to the first item in the list.</summary>
			public void Reset()
			{
				this.list.marker = this.list.head;
			}

			/// <summary>Return the item at the current marker position.</summary>
			public object Current
			{
				get { return this.list.marker.Item; }
			}

		}

		/// <summary>Reference to the first item that was inserted into the list.</summary>
		private ListItem head = null;

		/// <summary>Reference to the current list item.</summary>
		private ListItem marker = null;

		/// <summary>Number of items currently in the list.</summary>
		private int length = 0;

		/// <summary>Instantiates a new instance of a circular list.</summary>
		public CircularList()
		{
			// initialize all the pointers to be null
			this.marker = null;
			this.head = null;
		}

		/// <summary>Instantiates a new instance of a circular list with the specified item inserted.</summary>
		public CircularList( object x )
		{
			this.Add( x );
		}

		public int Length
		{
			get { return this.length; }
		}

		/// <summary>Returns an enumerator the the circular list.</summary>
		public IEnumerator GetEnumerator()
		{
			return (IEnumerator)new CircularListEnumerator( this );
		}

		/// <summary>Inserts a new item after the position of the marker.  Sets the new item to point to the item that was after the marker before the insertion.</summary>
		/// <param name="x">Object to be inserted into the list.</param>
		public void Add( object x )
		{
			// if the list is empty, create a new list item and set it's next to itself
			if ( this.IsEmpty() )
			{
				// create a new list item that points to itself
				this.marker = new ListItem( x, null, null );
				this.marker.Next = this.marker;
				this.marker.Previous = this.marker;
				this.head = this.marker;
			}
			// insert the new item at the end of the list
			else
			{
				// inserts a new item at the end of the list
				ListItem tail = this.head.Previous;
				this.head.Previous = new ListItem( x, tail.Next, tail );
				tail.Next = this.head.Previous;
			}

			// add one to the length of the list
			++this.length;
		}

		/// <summary>Delete the item at the marker position and return the deleted item.</summary>
		public object Delete()
		{
			if ( this.IsEmpty() )
				return null;

			// remeber the object at the current position
			object temp = this.marker.Item;

			// if there is only one item in the list, remove it
			if ( this.marker.Next == this.marker )
				this.marker = null;
			else
			{
				this.marker.Previous.Next = this.marker.Next;
				this.marker.Next.Previous = this.marker.Previous;
			}

			// subtract one to the length of the list
			--this.length;

			// return the removed item
			return temp;

		}

		/// <summary>Advance the marker one item ahead in the list.</summary>
		public void MoveNext()
		{
			// if the list is empty, return null
			if ( this.IsEmpty() )
				return;

			// advance the marker
			this.marker = this.marker.Next;
		}

		/// <summary>Move the marker one item back in the list.</summary>
		public void MovePrevious()
		{
			// if the list is empty, return null
			if ( this.IsEmpty() )
				return;

			// advance the marker
			this.marker = this.marker.Previous;
		}

		/// <summary>Resets the current marker to the first item in the list.</summary>
		public void Reset()
		{
			this.marker = this.head;
		}

		/// <summary>Return the item at the current marker position.</summary>
		public object Current
		{
			get { return this.marker.Item; }
		}

		/// <summary>Determines if the list is empty or not.</summary>
		/// <returns>True if the list is empty, otherwise false.</returns>
		public bool IsEmpty()
		{
			return ( this.marker == null );
		}
	}
}
