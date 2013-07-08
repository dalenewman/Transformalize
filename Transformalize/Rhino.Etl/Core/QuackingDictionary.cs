using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Transformalize.Rhino.Etl.Core.Exceptions;

namespace Transformalize.Rhino.Etl.Core {
    /// <summary>
    /// A dictionary that can be access with a natural syntax from Boo
    /// </summary>
    [Serializable]
    public class QuackingDictionary : IDictionary<string, object> {
        /// <summary>
        /// The inner items collection
        /// </summary>
        protected IDictionary<string, object> Items;

        /// <summary>
        /// The last item that was access, useful for debugging
        /// </summary>
        protected string LastAccess;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuackingDictionary"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public QuackingDictionary(IEnumerable<KeyValuePair<string, object>> items) {
            Items = items != null ?
                new ConcurrentDictionary<string, object>(items, StringComparer.InvariantCultureIgnoreCase) :
                new ConcurrentDictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
        }

        public bool TryGetValue(string key, out object value) {
            return Items.TryGetValue(key, out value);
        }

        public object this[string key] {
            get {
                LastAccess = key;
                //return items.ContainsKey(key) ? items[key] : null;
                return Items[key];
            }
            set {
                LastAccess = key;
                if (value == DBNull.Value)
                    Items[key] = null;
                else
                    Items[key] = value;
            }
        }

        /// <summary>
        /// Get a value by name or first parameter
        /// </summary>
        public virtual object QuackGet(string name, object[] parameters) {
            if (parameters == null || parameters.Length == 0)
                return this[name];
            if (parameters.Length == 1)
                return this[(string)parameters[0]];
            throw new ParameterCountException("You can only call indexer with a single parameter");
        }

        /// <summary>
        /// Set a value on the given name or first parameter
        /// </summary>
        public object QuackSet(string name, object[] parameters, object value) {
            if (parameters == null || parameters.Length == 0)
                return this[name] = value;
            if (parameters.Length == 1)
                return this[(string)parameters[0]] = value;
            throw new ParameterCountException("You can only call indexer with a single parameter");
        }

        public object QuackInvoke(string name, params object[] args) {
            throw new NotSupportedException(
                "You cannot invoke methods on a row, it is merely a data structure, after all.");
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() {
            return Items.GetEnumerator();
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append("{");
            foreach (var pair in Items) {
                sb.Append(pair.Key)
                    .Append(" : ");
                if (pair.Value is string) {
                    sb.Append("\"")
                        .Append(pair.Value)
                        .Append("\"");
                }
                else {
                    sb.Append(pair.Value);
                }
                sb.Append(", ");

            }
            sb.Append("}");
            return sb.ToString();
        }

        public IEnumerator GetEnumerator() {
            return new ConcurrentDictionary<string, object>(Items).GetEnumerator();
        }

        public bool Contains(string key) {
            return Items.ContainsKey(key);
        }

        public bool ContainsKey(string key) {
            return Items.ContainsKey(key);
        }

        public void Add(string key, object value) {
            Items.Add(key, value);
        }

        public bool Remove(string key) {
            return Items.Remove(key);
        }

        public void Add(KeyValuePair<string, object> item) {
            Items.Add(item);
        }

        public void Clear() {
            Items.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item) {
            return Items.Contains(item);
        }

        public ICollection<string> Keys {
            get { return Items.Keys; }
        }

        public ICollection<object> Values {
            get { return Items.Values; }
        }

        public bool IsReadOnly {
            get { return Items.IsReadOnly; }
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int index) {
            Items.CopyTo(array, index);
        }

        public bool Remove(KeyValuePair<string, object> item) {
            return Items.Remove(item);
        }

        public int Count {
            get { return Items.Count; }
        }

    }
}