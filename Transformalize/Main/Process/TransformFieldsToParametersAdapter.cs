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
using Transformalize.Configuration;

namespace Transformalize.Main {

    public class TransformFieldsToParametersAdapter {
        private readonly ProcessConfigurationElement _process;

        public TransformFieldsToParametersAdapter(ProcessConfigurationElement process) {
            _process = process;
        }

        public int Adapt(string transformName) {
            var count = 0;
            foreach (EntityConfigurationElement entity in _process.Entities) {
                foreach (FieldConfigurationElement field in entity.Fields) {
                    foreach (TransformConfigurationElement transform in field.Transforms) {
                        if (!transform.Method.Equals(transformName, StringComparison.OrdinalIgnoreCase)) continue;

                        foreach (FieldConfigurationElement tField in transform.Fields) {
                            transform.Parameters.Add(new ParameterConfigurationElement {
                                Entity = entity.Alias,
                                Field = tField.Alias,
                                Name = tField.Name
                            });
                            count++;
                        }
                    }
                }
            }
            return count;
        }
    }
}