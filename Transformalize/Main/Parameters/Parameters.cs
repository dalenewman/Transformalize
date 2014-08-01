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

using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Transformalize.Main.Parameters {
    public class Parameters : IParameters, IEnumerable<KeyValuePair<string, IParameter>> {

        private readonly DefaultFactory _defaultFactory = new DefaultFactory();
        private readonly IDictionary<string, IParameter> _items = new Dictionary<string, IParameter>();
        private KeyValuePair<string, IParameter> _first;
        private int _index = 0;

        IEnumerator<KeyValuePair<string, IParameter>> IEnumerable<KeyValuePair<string, IParameter>>.GetEnumerator() {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public int Count {
            get { return _items.Count; }
        }

        public IEnumerable<string> Keys {
            get { return _items.Keys; }
        }

        public IEnumerator<KeyValuePair<string, IParameter>> GetEnumerator() {
            return _items.GetEnumerator();
        }

        public IParameter this[string key] {
            get { return _items[key]; }
            set {
                _items[key] = value;
                RecordFirst(key);
            }
        }

        public IParameter this[int index] {
            get { return _items.Where(i => i.Value.Index.Equals(index)).Select(i => i.Value).First(); }
        }

        public void Add(string field, string name, object value, string type) {
            var parameter = new Parameter {
                Name = name,
                Value = value == null ? null : _defaultFactory.Convert(value, type),
                SimpleType = Common.ToSimpleType(type),
            };
            Add(field, parameter);
        }

        public KeyValuePair<string, IParameter> First() {
            return _first;
        }

        public bool Any() {
            return _first.Key != null;
        }

        public IEnumerable<KeyValuePair<string, IParameter>> ToEnumerable() {
            return _items.Select(p => p).OrderBy(p => p.Value.Index);
        }

        public bool ContainsKey(string key) {
            return _items.ContainsKey(key);
        }

        public void Remove(string key) {
            _items.Remove(key);
        }

        public ExpandoObject ToExpandoObject() {
            var parameters = new ExpandoObject();
            foreach (var parameter in this) {
                ((IDictionary<string, object>)parameters).Add(parameter.Value.Name, parameter.Value.Value);
            }
            return parameters;
        }

        public void Add(string field, IParameter parameter) {
            if (_items.ContainsKey(field)) {
                _items[field] = parameter;
            } else {
                parameter.Index = _index;
                _items.Add(field, parameter);
                RecordFirst(field, parameter);
                _index++;
            }
        }

        private void RecordFirst(string field, IParameter parameter) {
            if (_first.Key == null) {
                _first = new KeyValuePair<string, IParameter>(field, parameter);
            }
        }

        private void RecordFirst(string key) {
            if (_first.Key == null) {
                _first = new KeyValuePair<string, IParameter>(key, _items[key]);
                ;
            }
        }

        public void AddRange(IParameters parameters) {
            foreach (var parameter in parameters) {
                Add(parameter.Key, parameter.Value);
            }
        }
    }
}