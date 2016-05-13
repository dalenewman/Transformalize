#region license
// Transformalize
// A Configurable ETL solution specializing in incremental denormalization.
// Copyright 2013 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline {
    public class SlaveRow : IRow {

        public object[] Storage { get; set; }

        public SlaveRow(int capacity) {
            Storage = new object[capacity];
        }

        public object this[IField field] {
            get { return Storage[field.Index]; }
            set { Storage[field.Index] = value; }
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