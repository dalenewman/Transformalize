using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations.Transform {
    public class SubstringOperation : AbstractOperation {
        private readonly string _inKey;
        private readonly string _outKey;
        private readonly int _startIndex;
        private readonly int _length;

        public SubstringOperation(string inKey, string outKey, int startIndex, int length) {
            _inKey = inKey;
            _outKey = outKey;
            _startIndex = startIndex;
            _length = length;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                var value = row[_inKey].ToString();
                if (_startIndex < value.Length) {
                    if (_length == 0) {
                        row[_outKey] = value.Substring(_startIndex);
                    } else {
                        row[_outKey] = value.Substring(_startIndex, _length);
                    }
                }

                yield return row;
            }
        }
    }
}