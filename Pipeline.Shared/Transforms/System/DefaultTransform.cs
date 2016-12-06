#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.System {
    public class DefaultTransform : BaseTransform {
        class FieldDefault : IField {

            private readonly Action<IRow> _setter;
            public string Name { get; }
            public string Alias { get; }
            public short Index { get; }
            public short MasterIndex { get; }
            public short KeyIndex { get; }
            public string Type { get; }
            public object Value { private get; set; }
            public string StringValue { private get; set; }
            public bool DefaultWhiteSpace { get; set; }

            public FieldDefault(string name, string alias, short index, short masterIndex, short keyIndex, string type)
            {
                Name = name;
                Alias = alias;
                Index = index;
                MasterIndex = masterIndex;
                Type = type;
                KeyIndex = keyIndex;
                if (type == "string") {
                    _setter = row => row[this] = StringValue;
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
            : base(context, null) {
            var expanded = fields.ToArray();
            var defaults = Constants.TypeDefaults();

            foreach (var field in expanded) {
                var hasDefault = field.Default != Constants.DefaultSetting;

                if (field.Type.StartsWith("date")) {
                    switch (field.Default.ToLower()) {
                        case "now()":
                            field.Default = DateTime.UtcNow.ToString("O");
                            break;
                        case "today()":
                            field.Default = DateTime.Today.ToUniversalTime().ToString("O");
                            break;
                    }
                }

                var fieldDefault = new FieldDefault(field.Name, field.Alias, field.Index, field.MasterIndex, field.KeyIndex, field.Type) {
                    Value = hasDefault ?
                        field.Convert(field.Default) :
                        defaults[field.Type],
                    StringValue = hasDefault ? field.Default : string.Empty,
                    DefaultWhiteSpace = field.DefaultWhiteSpace
                };

                if (field.IsCalculated) {
                    CalculatedFieldDefaults.Add(fieldDefault);
                } else {
                    FieldDefaults.Add(fieldDefault);
                }
            }
        }

        public override IRow Transform(IRow row) {
            foreach (var field in CalculatedFieldDefaults) {
                field.Setter(row);
            }
            foreach (var field in FieldDefaults.Where(f => row[f] == null || f.DefaultWhiteSpace && f.Type == "string" && string.IsNullOrWhiteSpace((string)row[f]))) {
                field.Setter(row);
            }
            // Increment();
            return row;
        }

    }
}