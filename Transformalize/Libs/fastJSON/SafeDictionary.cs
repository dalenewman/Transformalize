#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System.Collections.Generic;

namespace Transformalize.Libs.fastJSON
{
    internal sealed class SafeDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _Dictionary;
        private readonly object _Padlock = new object();

        public SafeDictionary(int capacity)
        {
            _Dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        public SafeDictionary()
        {
            _Dictionary = new Dictionary<TKey, TValue>();
        }

        public int Count
        {
            get { lock (_Padlock) return _Dictionary.Count; }
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (_Padlock)
                    return _Dictionary[key];
            }
            set
            {
                lock (_Padlock)
                    _Dictionary[key] = value;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_Padlock)
                return _Dictionary.TryGetValue(key, out value);
        }

        public void Add(TKey key, TValue value)
        {
            lock (_Padlock)
            {
                if (_Dictionary.ContainsKey(key) == false)
                    _Dictionary.Add(key, value);
            }
        }
    }
}