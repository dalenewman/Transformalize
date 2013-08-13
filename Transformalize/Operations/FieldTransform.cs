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
using Transformalize.Core.Transform_;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations
{
    public class FieldTransform : AbstractOperation
    {
        private readonly IFields _fields;

        public FieldTransform(Entity entity)
        {
            _fields = new FieldSqlWriter(entity.All).ExpandXml().HasTransform().Input().Context();
            UseTransaction = false;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            foreach (var row in rows)
            {
                foreach (var field in _fields)
                {
                    var value = row[field.Key];

                    if (field.Value.UseStringBuilder)
                    {
                        field.Value.StringBuilder.Clear();
                        field.Value.StringBuilder.Append(value);
                        field.Value.Transform();
                        row[field.Key] = field.Value.StringBuilder.ToString();
                    }
                    else
                    {
                        field.Value.Transform(ref value);
                        row[field.Key] = value;
                    }
                }
                yield return row;
            }
        }
    }

}