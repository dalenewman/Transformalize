using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Transformalize.Rhino.Etl.Core.Exceptions;
using Enumerable = System.Linq.Enumerable;

namespace Transformalize.Rhino.Etl.Core {
    /// <summary>
    /// A dictionary that can be access with a natural syntax from Boo
    /// </summary>
    [Serializable]
    public class QuackingDictionary : IDictionary<string, object> {
        /// <summary>
        /// The inner items collection
        /// </summary>
        protected IDictionary<string, object> items;

        /// <summary>
        /// The last item that was access, useful for debugging
        /// </summary>
        protected string lastAccess;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuackingDictionary"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public QuackingDictionary(IDictionary<string, object> items) {
            this.items = items != null ?
                new Dictionary<string, object>(items, StringComparer.InvariantCultureIgnoreCase) :
                new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
        }

        public bool TryGetValue(string key, out object value) {
            return items.TryGetValue(key, out value);
        }

        public object this[string key] {
            get {
                lastAccess = key;
                return items.ContainsKey(key) ? items[key] : null;
            }
            set {
                lastAccess = key;
                if (value == DBNull.Value)
                    items[key] = null;
                else
                    items[key] = value;
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

        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append("{");
            foreach (var pair in items) {
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
            return new Dictionary<string, object>(items).GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() {
            return items.GetEnumerator();
        }

        public bool Contains(string key) {
            return items.ContainsKey(key);
        }

        public bool ContainsKey(string key) {
            return items.ContainsKey(key);
        }

        public void Add(string key, object value) {
            items.Add(key, value);
        }

        public bool Remove(string key) {
            return items.Remove(key);
        }

        public void Add(KeyValuePair<string, object> item) {
            items.Add(item);
        }

        public void Clear() {
            items.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item) {
            return items.Contains(item);
        }

        public ICollection<string> Keys {
            get { return items.Keys; }
        }

        public ICollection<object> Values {
            get { return items.Values; }
        }

        public bool IsReadOnly {
            get { return items.IsReadOnly; }
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int index) {
            items.CopyTo(array, index);
        }

        public bool Remove(KeyValuePair<string, object> item) {
            return items.Remove(item);
        }

        public int Count {
            get { return items.Count; }
        }

    }
}