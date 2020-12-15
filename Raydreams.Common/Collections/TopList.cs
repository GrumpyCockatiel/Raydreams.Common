using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Raydreams.Common.Collections
{
	/// <summary></summary>
	/// <typeparam name="T"></typeparam>
	public class TopList<T> : IEnumerable<T>
	{
		private List<T> _list = new List<T>();

		private Func<T, int> _comparer;

		public TopList(int size, Func<T,int> compare)
		{
			if ( size < 1 )
				size = 1;

			this._list = new List<T>( size );
			this._comparer = compare;
		}
		public void Add(T item)
		{
			int curMin = this._list.Min( this._comparer );
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this._list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

	}
}
