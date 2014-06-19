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

using System.Collections.Generic;

namespace Transformalize.Main.Providers {
    public enum StarFieldType {
        Master,
        Foreign,
        Other
    }

    public class StarFields {
        private readonly Process _process;

        public StarFields(Process process) {
            _process = process;
        }

        public Dictionary<StarFieldType, Fields> TypedFields() {
            var fields = new Dictionary<StarFieldType, Fields>();

            fields[StarFieldType.Master] = new Fields();
            fields[StarFieldType.Foreign] = new Fields();
            fields[StarFieldType.Other] = new Fields();

            foreach (var entity in _process.Entities) {
                if (entity.IsMaster()) {
                    fields[StarFieldType.Master].Add(new Fields(entity.Fields, entity.CalculatedFields, _process.CalculatedFields).WithOutput());
                } else {
                    if (entity.Fields.WithForeignKey().Any()) {
                        fields[StarFieldType.Foreign].Add(entity.Fields.WithOutput().WithForeignKey());
                    }
                    fields[StarFieldType.Other].Add(new Fields(entity.Fields, entity.CalculatedFields).WithOutput().WithoutBytes().WithoutKey());
                }
            }

            return fields;
        }

    }
}