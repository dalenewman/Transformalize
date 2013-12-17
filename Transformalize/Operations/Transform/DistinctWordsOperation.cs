using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations.Transform {
    public class DistinctWordsOperation : AbstractOperation {

        private readonly string _inKey;
        private readonly string _outKey;
        private readonly string _separator;
        private readonly char[] _separatorArray;
        private readonly bool _inOutDifferent = true;

        public DistinctWordsOperation(string inKey, string outKey, string separator) {
            _inKey = inKey;
            _outKey = outKey;
            _separator = separator;
            _separatorArray = separator.ToCharArray();
            _inOutDifferent = !_inKey.Equals(_outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                var value = row[_inKey].ToString();
                if (value.Contains(_separator)) {
                    row[_outKey] = string.Join(_separator, value.Split(_separatorArray, StringSplitOptions.RemoveEmptyEntries).Distinct());
                } else if (_inOutDifferent) {

                    row[_outKey] = row[_inKey];
                }

                yield return row;
            }
        }
    }
}