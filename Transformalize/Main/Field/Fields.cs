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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Transformalize.Main {
    public class Fields : IEnumerable<KeyValuePair<string, Field>> {
        private readonly IDictionary<string, Field> _items = new Dictionary<string, Field>();

        public Fields() {
        }

        public Fields(IEnumerable<Field> fields) {
            AddRange(fields);
        }

        public Fields(Dictionary<string, Field> fields) {
            AddRange(fields);
        }

        public Fields(Process process, IParameters parameters) {
            foreach (var parameter in parameters) {
                if (parameter.Value.HasValue()) {
                    var field = new Field(parameter.Value.SimpleType, "64", FieldType.Field, false, parameter.Value.Value.ToString()) {
                        Alias = parameter.Value.Name,
                    };
                    Add(field);
                } else {
                    Add(process.GetField(parameter.Value.Name));
                }
            }
        }

        IEnumerator<KeyValuePair<string, Field>> IEnumerable<KeyValuePair<string, Field>>.GetEnumerator() {
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

        public IEnumerator<KeyValuePair<string, Field>> GetEnumerator() {
            return _items.GetEnumerator();
        }

        public IEnumerable<Field> ToEnumerable() {
            return _items.Select(kv => kv.Value).OrderBy(f => f.Alias);
        }

        public Field this[string key] {
            get { return _items[key]; }
            set { _items[key] = value; }
        }

        public void Add(string key, Field field) {
            _items[key] = field;
        }

        public void AddRange(Fields fields) {
            foreach (var field in fields) {
                Add(field);
            }
        }

        public bool Any() {
            return _items.Any();
        }

        public bool Any(Func<KeyValuePair<string, Field>, bool> predicate) {
            return _items.Any(predicate);
        }

        public bool ContainsKey(string key) {
            return _items.ContainsKey(key);
        }

        public void Remove(string key) {
            _items.Remove(key);
        }

        public KeyValuePair<string, Field> First() {
            return _items.First();
        }

        public KeyValuePair<string, Field> First(Func<KeyValuePair<string, Field>, bool> predicate) {
            return _items.First(predicate);
        }

        public KeyValuePair<string, Field> Last() {
            return _items.Last();
        }

        public KeyValuePair<string, Field> Last(Func<KeyValuePair<string, Field>, bool> predicate) {
            return _items.Last(predicate);
        }

        public void Add(Field field) {
            _items[field.Alias] = field;
        }

        public void Add(KeyValuePair<string, Field> field) {
            _items[field.Key] = field.Value;
        }

        public void AddRange(IEnumerable<Field> fields) {
            foreach (var field in fields) {
                Add(field);
            }
        }

        public void AddRange(Dictionary<string, Field> fields) {
            foreach (var field in fields) {
                Add(field);
            }
        }
    }
}