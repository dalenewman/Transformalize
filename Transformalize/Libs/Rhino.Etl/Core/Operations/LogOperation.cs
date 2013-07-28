using System;
using System.Collections.Generic;
using System.Linq;

namespace Transformalize.Libs.Rhino.Etl.Core.Operations {
    public class LogOperation : AbstractOperation {

        private readonly string _delimiter;
        private readonly int _maxLengh;
        private readonly List<string> _ignores = new List<string>();
        private KeyValuePair<string, object> _whereFilter;

        public LogOperation(string delimiter = " | ", int maxLength = 64) {
            _delimiter = delimiter;
            _maxLengh = maxLength;
            _whereFilter = new KeyValuePair<string, object>(string.Empty, null);
        }

        public LogOperation Ignore(string column) {
            _ignores.Add(column);
            return this;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (!String.IsNullOrEmpty(_whereFilter.Key)) {
                    if (row[_whereFilter.Key] == _whereFilter.Value) {
                        LogRow(row);
                    }
                } else {
                    LogRow(row);
                }

                yield return row;
            }
        }

        private void LogRow(Row row) {
            var values = row.Columns.Where(column => !_ignores.Contains(column)).Select(column => EnforceMaxLength(row[column])).ToList();
                Info(string.Join(_delimiter, values));
        }

        public string EnforceMaxLength(object value) {
            if (value == null)
                return string.Empty;

            var stringValue = value.ToString().Replace(Environment.NewLine, " ");

            if (stringValue.Length > _maxLengh)
                return stringValue.Substring(0, _maxLengh - 3) + "...";

            return stringValue;
        }

        public IOperation Where(string key, object value) {
            _whereFilter = new KeyValuePair<string, object>(key, value);
            return this;
        }
    }
}
