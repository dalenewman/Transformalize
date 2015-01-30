#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.Collections.Generic;
using Transformalize.Configuration;

namespace Transformalize.Main {

    public class TransformFieldsToParametersAdapter {
        private readonly TflProcess _process;

        public TransformFieldsToParametersAdapter(TflProcess process) {
            _process = process;
        }

        public int Adapt(string transformName) {
            var count = 0;
            foreach (TflEntity entity in _process.Entities) {
                count += AddParameters(entity.Fields, transformName, entity.Alias);
                count += AddParameters(entity.CalculatedFields, transformName, entity.Alias);
            }
            return count;
        }

        public int AddParameters(List<TflField> fields, string transformName, string entity) {
            var count = 0;
            foreach (var field in fields) {
                foreach (var transform in field.Transforms) {
                    if (!transform.Method.Equals(transformName, StringComparison.OrdinalIgnoreCase)) continue;

                    for (var i = 0; i < transform.Fields.Count; i++) {
                        var tField = transform.Fields[i];
                        transform.Parameters.Add(tField.GetDefaultOf<TflParameter>(p => {
                            p.Entity = entity;
                            p.Field = tField.Alias;
                            p.Name = tField.Name;
                            p.Input = false;
                        }));
                        count++;
                    }
                }
            }
            return count;
        }
    }
}