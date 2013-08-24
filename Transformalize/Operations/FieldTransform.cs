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
using Transformalize.Core.Entity_;
using Transformalize.Core.Field_;
using Transformalize.Core.Fields_;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;
using System.Linq;

namespace Transformalize.Operations
{
    public class FieldTransform : AbstractOperation
    {
        private readonly Field[] _fields;
        private readonly int _transformCount;

        public FieldTransform(Entity entity)
        {
            _fields = new FieldSqlWriter(entity.All).ExpandXml().HasTransform().Input().ToArray();
            _transformCount = _fields.Any() ? _fields.Sum(f => f.Transforms.Count) : 0;
            UseTransaction = false;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            foreach (var row in rows)
            {
                if (_transformCount > 0)
                {
                    foreach (var field in _fields)
                    {
                        var value = row[field.Alias];
                        if (value == null) continue;

                        if (field.UseStringBuilder)
                        {
                            field.StringBuilder.Clear();
                            field.StringBuilder.Append(value);
                            field.Transform();
                            row[field.Alias] = field.StringBuilder.ToString();
                        }
                        else
                        {
                            field.Transform(ref value);
                            row[field.Alias] = value;
                        }
                    }
                }

                yield return row;
            }
        }
    }

}