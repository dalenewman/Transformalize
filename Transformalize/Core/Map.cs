/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections;
using System.Collections.Generic;

namespace Transformalize.Core
{
    public class Map : IEnumerable<KeyValuePair<string, Item>>
    {

        private readonly IDictionary<string, Item> _items = new Dictionary<string, Item>();

        public int Count { get { return _items.Count; } }

        public IEnumerable<string> Keys { get { return _items.Keys; } }

        public IEnumerator<KeyValuePair<string, Item>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public Item this[string key]
        {
            get
            {
                return _items[key];
            }
            set
            {
                _items[key] = value;
            }
        }

        public void Add(string key, Item item)
        {
            _items.Add(key, item);
        }

        IEnumerator<KeyValuePair<string, Item>> IEnumerable<KeyValuePair<string, Item>>.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool ContainsKey(string value)
        {
            return _items.ContainsKey(value);
        }
    }
}