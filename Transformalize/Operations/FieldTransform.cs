/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;
using Transformalize.Model;

namespace Transformalize.Operations {
    public class FieldTransform : AbstractOperation {
        private readonly Dictionary<string, Field> _fields;

        public FieldTransform(Entity entity) {
            _fields = new FieldSqlWriter(entity.All).ExpandXml().HasTransform().Input().Context();
            UseTransaction = false;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {

                foreach (var pair in _fields) {
                    var value = row[pair.Key];

                    if (pair.Value.UseStringBuilder) {
                        pair.Value.StringBuilder.Clear();
                        pair.Value.StringBuilder.Append(value);
                        foreach (var t in pair.Value.Transforms) {
                            t.Transform(ref pair.Value.StringBuilder);
                        }
                        row[pair.Key] = pair.Value.StringBuilder.ToString();
                    } else {
                        foreach (var t in pair.Value.Transforms) {
                            t.Transform(ref value);
                        }
                        row[pair.Key] = value;
                    }
                }
                yield return row;
            }
        }
    }

}