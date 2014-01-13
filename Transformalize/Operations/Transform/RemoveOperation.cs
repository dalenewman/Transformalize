using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class RemoveOperation : TflOperation {
        private readonly int _startIndex;
        private readonly int _length;

        public RemoveOperation(string inKey, string outKey, int startIndex, int length)
            : base(inKey, outKey) {
            _startIndex = startIndex;
            _length = length;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var value = row[InKey].ToString();
                    if (value.Length > _startIndex)
                        row[OutKey] = value.Remove(_startIndex, _length);
                }
                yield return row;
            }
        }
    }
}