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

namespace Transformalize.Libs.Rhino.Etl.Operations
{
    public class LogOperation : AbstractOperation
    {
        private readonly string _delimiter;
        private readonly List<string> _ignores = new List<string>();
        private readonly int _maxLengh;
        private readonly List<string> _only = new List<string>();
        private bool _firstRow = true;

        public LogOperation(string delimiter = " | ", int maxLength = 32)
        {
            _delimiter = delimiter;
            _maxLengh = maxLength;
        }

        public LogOperation Ignore(params string[] columns)
        {
            _ignores.AddRange(columns);
            return this;
        }

        public LogOperation Only(params string[] columns)
        {
            _only.AddRange(columns);
            return this;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            foreach (var row in rows)
            {
                if (IsDebugEnabled())
                {
                    if (_firstRow)
                    {
                        var columns = _only.Any()
                                          ? row.Columns.Where(column => _only.Contains(column))
                                               .Select(column => column.PadLeft(_maxLengh))
                                               .ToArray()
                                          : row.Columns.Where(column => !_ignores.Contains(column))
                                               .Select(column => column.PadLeft(_maxLengh))
                                               .ToArray();

                        Debug(new String('-', (columns.Count()*_maxLengh) + columns.Count() - 1));
                        Debug(string.Join(_delimiter, columns));
                    }

                    var values = _only.Any() ?
                                     row.Columns.Where(column => _only.Contains(column)).Select(column => EnforceMaxLength(row[column]).PadLeft(_maxLengh, ' ')).ToList() :
                                     row.Columns.Where(column => !_ignores.Contains(column)).Select(column => EnforceMaxLength(row[column]).PadLeft(_maxLengh, ' ')).ToList();

                    Debug(string.Join(_delimiter, values));

                    _firstRow = false;
                }
                yield return row;
            }
        }

        public string EnforceMaxLength(object value)
        {
            if (value == null)
                return string.Empty;

            var stringValue = value.ToString().Replace(Environment.NewLine, " ");

            if (stringValue.Length > _maxLengh)
                return stringValue.Substring(0, _maxLengh - 3) + "...";

            return stringValue;
        }
    }
}