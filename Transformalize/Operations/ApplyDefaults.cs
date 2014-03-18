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
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations {
    public class ApplyDefaults : AbstractOperation {
        private readonly Field[] _fields;
        private readonly bool _defaultNulls;

        public ApplyDefaults(bool defaultNulls, params Fields[] fields) {
            _fields = PrepareFields(fields);
            UseTransaction = false;
            _defaultNulls = defaultNulls;
        }

        private static Field[] PrepareFields(IEnumerable<Fields> fields) {
            var list = new List<Field>();
            foreach (var fieldArray in fields) {
                list.AddRange(new FieldSqlWriter(fieldArray).ToArray()); //HasDefault()
            }
            return list.ToArray();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                foreach (var field in _fields) {
                    var obj = row[field.Alias];
                    if ((_defaultNulls || !field.Input) && obj == null) {
                        row[field.Alias] = field.Default;
                    } else if ((field.DefaultBlank || field.DefaultWhiteSpace) && obj != null) {
                        if (field.DefaultBlank && obj.Equals(string.Empty))
                            row[field.Alias] = field.Default;
                        else if (IsWhiteSpace(obj.ToString())) {
                            row[field.Alias] = field.Default;
                        }
                    }
                }
                yield return row;
            }
        }

        public static bool IsWhiteSpace(string value) {
            return value.All(char.IsWhiteSpace);
        }
    }
}