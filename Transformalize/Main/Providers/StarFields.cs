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

namespace Transformalize.Main.Providers
{
    public enum StarFieldType
    {
        Master,
        Foreign,
        Other
    }

    public class StarFields
    {
        private readonly Process _process;

        public StarFields(Process process)
        {
            _process = process;
        }

        public Dictionary<StarFieldType, IFields> TypedFields() {
            var fields = new Dictionary<StarFieldType, IFields>();

            fields[StarFieldType.Master] = new Fields();
            fields[StarFieldType.Foreign] = new Fields();
            fields[StarFieldType.Other] = new Fields();

            foreach (var entity in _process.Entities) {
                if (entity.IsMaster()) {
                    fields[StarFieldType.Master].AddRange(new FieldSqlWriter(entity.PrimaryKey, entity.Fields, _process.CalculatedFields, entity.CalculatedFields).ExpandXml().Output().Context());
                } else {
                    if (entity.Fields.Any(f => f.Value.FieldType.HasFlag(FieldType.ForeignKey))) {
                        fields[StarFieldType.Foreign].AddRange(new FieldSqlWriter(entity.Fields).ExpandXml().Output().FieldType(FieldType.ForeignKey).Context());
                    }
                    fields[StarFieldType.Other].AddRange(new FieldSqlWriter(entity.Fields, entity.CalculatedFields).ExpandXml().Output().FieldType(FieldType.Field, FieldType.Version, FieldType.Xml).Context());
                }
            }

            return fields;
        }

        public IEnumerable<Field> Fields()
        {
            var fields = new List<Field>();
            var typedFields = TypedFields();
            foreach (var pair in typedFields)
            {
                var pairFields = typedFields[pair.Key];
                if (pairFields.Any())
                {
                    fields.AddRange(pairFields.ToEnumerable());
                }
            }
            return fields;
        }
        
    }
}