using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations.Transform
{
    public class InsertOperation : AbstractOperation {
        private readonly string _inKey;
        private readonly string _outKey;
        private readonly int _startIndex;
        private readonly string _value;

        public InsertOperation(string inKey, string outKey, int startIndex, string value) {
            _inKey = inKey;
            _outKey = outKey;
            _startIndex = startIndex;
            _value = value;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                var value = row[_inKey].ToString();
                if (_startIndex > value.Length)
                    row[_outKey] = value.Insert(_startIndex, _value);
                yield return row;
            }
        }
    }
}