using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Transformalize.Libs.Rhino.Etl.Core {
    /// <summary>
    /// Represent a virtual row
    /// </summary>
    [DebuggerDisplay("Count = {items.Count}")]
    [Serializable]
    public class Row : IEnumerable
    {
        private static readonly StringComparer Ic = StringComparer.InvariantCultureIgnoreCase;
        private IDictionary<string, object> _storage;

        /// <summary>
        /// Initializes a new instance of the <see cref="Row"/> class.
        /// </summary>
        public Row(){
             _storage =  new Dictionary<string, object>(Ic);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Row"/> class.
        /// </summary>
        /// <param name="itemsToClone">The items to clone.</param>
        protected Row(IDictionary<string, object> itemsToClone)
        {
            _storage = new Dictionary<string, object>( itemsToClone, Ic);
        }


        /// <summary>
        /// Creates a copy of the given source, erasing whatever is in the row currently.
        /// </summary>
        /// <param name="source">The source row.</param>
        public void Copy(IDictionary<string, object> source) {
            _storage = new Dictionary<string, object>(source, Ic);
        }

        public IEnumerable<string> Columns {
            get {
                return new List<string>(_storage.Keys);
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public Row Clone() {
            return new Row(_storage);
        }

        public bool ContainsKey(string key)
        {
            return _storage.ContainsKey(key);
        }

        public bool Contains(string key)
        {
            return _storage.ContainsKey(key);
        }

        /// <summary>
        /// Creates a key that allow to do full or partial indexing on a row
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns></returns>
        public ObjectArrayKeys CreateKey(string[] columns) {
            var array = new object[columns.Length];
            for (var i = 0; i < columns.Length; i++) {
                array[i] = _storage[columns[i]];
            }
            return new ObjectArrayKeys(array);
        }

        public object this[string key]
        {
            get
            {
                return _storage.ContainsKey(key) ? _storage[key] : null;
            }
            set
            {
                if (value == DBNull.Value)
                    _storage[key] = null;
                else
                    _storage[key] = value;
            }
        }

        /// <summary>
        /// Generate a row from the reader
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static Row FromReader(IDataReader reader) {
            var row = new Row();
            var count = reader.FieldCount;
            for (var i = 0; i < count; i++) {
                row[reader.GetName(i)] = reader.GetValue(i);
            }
            return row;
        }

        public IEnumerator GetEnumerator()
        {
            return _storage.GetEnumerator();
            
        }

        public void Add(string key, object value)
        {
            _storage.Add(key, value);
        }

    }
}
