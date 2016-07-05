#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
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
using System;
using System.Collections.Generic;
using System.Linq;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms.System {
    public class DefaultTransform : BaseTransform, ITransform {
        class FieldDefault : IField {

            private readonly Action<IRow> _setter;
            public string Alias { get; }
            public short Index { get; }
            public short MasterIndex { get; }
            public short KeyIndex { get; }
            public string Type { get; }
            public object Value { private get; set; }
            public string StringValue { private get; set; }

            public bool DefaultWhiteSpace { get; set; }

            public FieldDefault(string alias, short index, short masterIndex, string type) {
                Alias = alias;
                Index = index;
                MasterIndex = masterIndex;
                Type = type;
                if (type == "string") {
                    _setter = row => row.SetString(this, StringValue);
                } else {
                    _setter = row => row[this] = Value;
                }
            }

            public void Setter(IRow row) {
                _setter(row);
            }
        }

        List<FieldDefault> FieldDefaults { get; } = new List<FieldDefault>();
        List<FieldDefault> CalculatedFieldDefaults { get; } = new List<FieldDefault>();

        public DefaultTransform(IContext context, IEnumerable<Field> fields)
            : base(context) {
            var expanded = fields.ToArray();

            foreach (var field in expanded) {
                var fieldDefault = new FieldDefault(field.Alias, field.Index, field.MasterIndex, field.Type) {
                    Value = field.Default == Constants.DefaultSetting ? Constants.TypeDefaults()[field.Type] : field.Convert(field.Default),
                    StringValue = field.Default == Constants.DefaultSetting ? string.Empty : field.Default,
                    DefaultWhiteSpace = field.DefaultWhiteSpace
                };
                if (field.IsCalculated) {
                    CalculatedFieldDefaults.Add(fieldDefault);
                } else {
                    FieldDefaults.Add(fieldDefault);
                }
            }
        }

        public IRow Transform(IRow row) {
            foreach (var field in CalculatedFieldDefaults) {
                field.Setter(row);
            }
            foreach (var field in FieldDefaults.Where(f => row[f] == null || f.DefaultWhiteSpace && f.Type == "string" && string.IsNullOrWhiteSpace((string)row[f]))) {
                field.Setter(row);
            }
            Increment();
            return row;
        }

    }
}