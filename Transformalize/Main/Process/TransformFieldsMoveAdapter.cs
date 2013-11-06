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
using System.Linq;
using Transformalize.Configuration;

namespace Transformalize.Main {
    public class TransformFieldsMoveAdapter {

        private readonly ProcessConfigurationElement _process;

        public TransformFieldsMoveAdapter(ProcessConfigurationElement process) {
            _process = process;
        }

        public int Adapt(string transformName)
        {
            var count = 0;
            var fields = new Dictionary<string, Dictionary<string, List<FieldConfigurationElement>>>();

            foreach (EntityConfigurationElement entity in _process.Entities) {

                fields[entity.Alias] = new Dictionary<string, List<FieldConfigurationElement>>();

                foreach (FieldConfigurationElement field in entity.Fields) {
                    foreach (TransformConfigurationElement transform in field.Transforms) {
                        if (!transform.Method.Equals(transformName, StringComparison.OrdinalIgnoreCase)) continue;

                        fields[entity.Alias][field.Alias] = new List<FieldConfigurationElement>();
                        foreach (FieldConfigurationElement tField in transform.Fields) {
                            tField.Input = false;
                            fields[entity.Alias][field.Alias].Add(tField);
                            count++;
                        }

                        transform.Fields.Clear();
                    }
                }
            }

            foreach (var entity in fields) {
                foreach (var field in entity.Value) {
                    var entityElement = _process.Entities.Cast<EntityConfigurationElement>().First(e => e.Alias == entity.Key);
                    var fieldElement = entityElement.Fields.Cast<FieldConfigurationElement>().First(f => f.Alias == field.Key);
                    var index = entityElement.Fields.IndexOf(fieldElement) + 1;
                    foreach (var element in field.Value) {
                        entityElement.Fields.InsertAt(element, index);
                        index++;
                    }
                }
            }
            return count;
        }
    }
}