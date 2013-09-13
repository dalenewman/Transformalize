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
using Transformalize.Main;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations
{
    public class ApplyDefaults : AbstractOperation
    {
        private readonly Field[] _fields;

        public ApplyDefaults(params IFields[] fields)
        {
            _fields = PrepareFields(fields);
            UseTransaction = false;
        }

        private static Field[] PrepareFields(IEnumerable<IFields> fields)
        {
            var list = new List<Field>();
            foreach (IFields fieldArray in fields)
            {
                list.AddRange(new FieldSqlWriter(fieldArray).ExpandXml().ToArray()); //HasDefault()
            }
            return list.ToArray();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            foreach (Row row in rows)
            {
                foreach (Field field in _fields)
                {
                    if (row[field.Alias] == null)
                    {
                        row[field.Alias] = field.Default;
                    }
                }
                yield return row;
            }
        }
    }
}