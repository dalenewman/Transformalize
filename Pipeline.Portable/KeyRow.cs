using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline
{
    public class KeyRow : IRow
    {
        public object[] Storage { get; set; }

        public KeyRow(int capacity) {
            Storage = new object[capacity];
        }

        public object this[IField field]
        {
            get { return Storage[field.KeyIndex]; }
            set { Storage[field.KeyIndex] = value; }
        }

        public ExpandoObject ToExpandoObject(Field[] fields) {
            var parameters = new ExpandoObject();
            var dict = ((IDictionary<string, object>)parameters);
            foreach (var field in fields) {
                dict.Add(field.FieldName(), this[field]);
            }
            return parameters;
        }

        public Dictionary<string, string> ToStringDictionary(Field[] fields) {
            return fields.ToDictionary(f => f.Alias, f => this[f].ToString());
        }

        public ExpandoObject ToFriendlyExpandoObject(Field[] fields) {
            var parameters = new ExpandoObject();
            var dict = ((IDictionary<string, object>)parameters);
            foreach (var field in fields) {
                dict.Add(field.Alias, this[field]);
            }
            return parameters;
        }

        public IEnumerable<object> ToEnumerable(Field[] fields) {
            return fields.Select(f => this[f]);
        }

        public bool Match(Field[] fields, IRow other) {
            return fields.Length > 1 ?
                fields.Select(f => this[f]).SequenceEqual(fields.Select(f => other[f])) :
                this[fields[0]].Equals(other[fields[0]]);
        }

        public override string ToString() {
            return string.Join("|", Storage);
        }

        public string GetString(IField f) {
            return this[f].ToString();
        }

        public void SetString(IField f, string value) {
            this[f] = value;
        }


    }
}