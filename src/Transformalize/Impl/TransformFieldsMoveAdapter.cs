#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
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

namespace Transformalize.Impl {

    public class TransformFieldsMoveAdapter {

        readonly Entity _entity;

        public TransformFieldsMoveAdapter(Entity entity) {
            _entity = entity;
        }

        public void Adapt(string transformName) {

            var fields = new Dictionary<string, Dictionary<string, List<Field>>>();
            var calculatedFields = new Dictionary<string, Dictionary<string, List<Field>>>();

            fields[_entity.Alias] = GetFields(_entity.Fields, transformName);
            RemoveFields(_entity.Fields, transformName);

            calculatedFields[_entity.Alias] = GetFields(_entity.CalculatedFields, transformName);
            RemoveFields(_entity.CalculatedFields, transformName);

            InsertAsCalculatedFields(fields);
            InsertAsCalculatedFields(calculatedFields);
        }

        public Dictionary<string, List<Field>> GetFields(List<Field> fields, string transformName) {
            var result = new Dictionary<string, List<Field>>();
            foreach (var field in fields) {
                foreach (var transform in field.Transforms) {
                    if (!transform.Method.Equals(transformName, StringComparison.OrdinalIgnoreCase))
                        continue;
                    result[field.Alias] = new List<Field>();
                    foreach (var tField in transform.Fields) {
                        tField.Input = false;
                        result[field.Alias].Add(tField);
                    }
                }
            }
            return result;
        }

        public void RemoveFields(List<Field> fields, string transformName) {
            foreach (var field in fields) {
                foreach (var transform in field.Transforms) {
                    if (!transform.Method.Equals(transformName, StringComparison.OrdinalIgnoreCase))
                        continue;
                    transform.Fields.Clear();
                }
            }
        }

        public void InsertAsCalculatedFields(Dictionary<string, Dictionary<string, List<Field>>> fields) {
            foreach (var pair in fields) {
                foreach (var field in pair.Value) {
                    var fieldElement = _entity.CalculatedFields.FirstOrDefault(f => f.Alias == field.Key);
                    var index = fieldElement == null ? 0 : _entity.CalculatedFields.IndexOf(fieldElement) + 1;
                    foreach (var element in field.Value) {
                        element.Produced = true;
                        _entity.CalculatedFields.Insert(index, element);
                        index++;
                    }
                }
            }
        }

    }
}