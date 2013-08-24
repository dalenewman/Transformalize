using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.NLog;

namespace Transformalize.Libs.Rhino.Etl.Core.Operations
{
    public class LogOperation : AbstractOperation
    {

        private readonly string _delimiter;
        private readonly int _maxLengh;
        private readonly List<string> _ignores = new List<string>();
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

                        Debug(new String('-', (columns.Count() * _maxLengh) + columns.Count() - 1));
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
