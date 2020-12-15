using System;
using System.Collections;
using System.Collections.Generic;

namespace Raydreams.Common.Collections
{
	/// <summary>A list of objects that are automatically shuffled when created.</summary>
	/// <typeparam name="T"></typeparam>
	public class ShuffledDeck<T> : IEnumerable<T>
	{
		private Random _rand = null;

		private List<T> _deck = new List<T>();

		/// <summary></summary>
		public ShuffledDeck(List<T> deck) : this(deck, null)
		{
		}

		/// <summary></summary>
		public ShuffledDeck(List<T> deck, Random generator)
		{
			this._rand = generator ?? new Random(Guid.NewGuid().GetHashCode());
			this.Deck = deck;
			this.Shuffle();
		}

		/// <summary></summary>
		protected List<T> Deck
		{
			set
			{
				this._deck = value ?? new List<T>();
			}
			get
			{
				return this._deck;
			}
		}

		/// <summary>Shuffles the current deck</summary>
		public void Shuffle()
		{
			if (this.Deck.Count < 2)
				return;

			for (int idx = 0; idx < this.Deck.Count-1; ++idx)
			{
				// pick a location
				int pick = this._rand.Next(idx+1, this.Deck.Count-1);

				// now swap the current and the random location
				T temp = this.Deck[idx];
				this.Deck[idx] = this.Deck[pick];
				this.Deck[pick] = temp;
			}
		}

		/// <summary></summary>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <summary></summary>
		public IEnumerator<T> GetEnumerator()
		{
			return this.Deck.GetEnumerator();
		}
	}
}
