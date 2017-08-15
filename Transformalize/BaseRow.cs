#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize {
    public abstract class BaseRow {
        public object[] Storage { get; set; }

        protected BaseRow(int capacity) {
            Storage = new object[capacity];
        }

        public abstract object GetValue(IField field);
        public abstract void SetValue(IField field, object value);

        public string GetStringValue(IField field) {
            return field.Type == "string" ? (string)GetValue(field) : GetValue(field).ToString();
        }

        public ExpandoObject ToExpandoObject(Field[] fields) {
            var parameters = new ExpandoObject();
            var dict = ((IDictionary<string, object>)parameters);
            foreach (var field in fields) {
                dict.Add(field.FieldName(), GetValue(field));
            }
            return parameters;
        }

        public CfgRow ToCfgRow(IField[] fields, string[] keys) {
            var row = new CfgRow(keys);
            for (var i = 0; i < fields.Length; i++) {
                row[keys[i]] = GetValue(fields[i]);
            }
            return row;
        }

        public ExpandoObject ToFriendlyExpandoObject(Field[] fields) {
            var parameters = new ExpandoObject();
            var dict = ((IDictionary<string, object>)parameters);
            foreach (var field in fields) {
                dict.Add(field.Alias, GetValue(field));
            }
            return parameters;
        }

        public IDictionary<string, object> ToFriendlyDictionary(Field[] fields) {
            return fields.ToDictionary(field => field.Alias, GetValue);
        }

        public IEnumerable<object> ToEnumerable(IEnumerable<Field> fields) {
            return fields.Select(GetValue);
        }

        public bool Match(Field[] fields, IRow other) {
            return fields.Length > 1 ?
                fields.Select(GetValue).SequenceEqual(fields.Select(f => other[f])) :
                GetValue(fields[0]).Equals(other[fields[0]]);
        }

        public override string ToString() {
            return string.Join("|", Storage.Select(i => i?.ToString() ?? string.Empty));
        }

        public object[] ToArray() {
            return Storage;
        }

    }
}