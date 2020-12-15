using System;
using System.Collections.Generic;

namespace Raydreams.Common.Collections
{
    /// <summary>ConsoleList</summary>
    /// <remarks>Not complete</remarks>
    public class ConsoleList
    {
        private int _max = 10;

        private List<string> _list = null;

        public ConsoleList(int maxLength)
        {
            this.MaxLength = maxLength;
            this._list = new List<string>();
        }

        public int MaxLength
        {
            get { return this._max; }
            set
            {
                if (value < 1)
                    value = 1;

                this._max = value;
            }
        }

        public string Current
        {
            get
            {
                return String.Concat(this._list);
            }
        }

        public void Add(string s)
        {
            this._list.Insert(0, $"{s}\n");

            if (this._list.Count > this.MaxLength)
                this._list.RemoveRange(this.MaxLength, this._list.Count - this.MaxLength);
        }

    }
}
