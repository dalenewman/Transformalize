using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class DistinctWordsOperation : ShouldRunOperation {

        private readonly string _separator;
        private readonly char[] _separatorArray;
        private readonly bool _inOutDifferent = true;

        public DistinctWordsOperation(string inKey, string outKey, string separator)
            : base(inKey, outKey) {
            _separator = separator;
            _separatorArray = separator.ToCharArray();
            _inOutDifferent = !inKey.Equals(outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var value = row[InKey].ToString();
                    if (value.Contains(_separator)) {
                        row[OutKey] = string.Join(_separator, value.Split(_separatorArray, StringSplitOptions.RemoveEmptyEntries).Distinct());
                    } else if (_inOutDifferent) {

                        row[OutKey] = row[InKey];
                    }
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}