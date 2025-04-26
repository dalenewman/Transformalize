#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
using Transformalize.Configuration;

namespace Transformalize.Impl {

    /// <summary>
    /// Producers are transforms that produce new fields as output.  The fields are originally defined
    /// within the transform, but get moved but into the calculated fields collection and remembered as 
    /// output parameters
    /// </summary>
    public class TransformFieldsToParametersAdapter {
        readonly Entity _entity;

        public TransformFieldsToParametersAdapter(Entity entity) {
            _entity = entity;
        }

        public int Adapt(string transformName) {
            var count = 0;
            count += AddParameters(_entity.Fields, transformName, _entity.Alias);
            count += AddParameters(_entity.CalculatedFields, transformName, _entity.Alias);
            return count;
        }

        public int AddParameters(List<Field> fields, string transformName, string entity) {
            var count = 0;
            foreach (var field in fields) {
                foreach (var transform in field.Transforms) {
                    if (!transform.Method.Equals(transformName, StringComparison.OrdinalIgnoreCase)) continue;

                    for (var i = 0; i < transform.Fields.Count; i++) {
                        var tField = transform.Fields[i];
                        transform.Parameters.Add(new Parameter {
                            Entity = entity,
                            Field = tField.Alias,
                            Name = tField.Name,
                            Input = false,
                            Type = tField.Type
                        });
                        count++;
                    }
                }
            }
            return count;
        }
    }
}