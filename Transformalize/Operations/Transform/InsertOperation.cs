using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class InsertOperation : ShouldRunOperation {
        private readonly int _startIndex;
        private readonly string _value;

        public InsertOperation(string inKey, string outKey, int startIndex, string value)
            : base(inKey, outKey) {
            _startIndex = startIndex;
            _value = value;
            Name = string.Format("Insert ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var value = row[InKey].ToString();
                    if (value.Length > _startIndex)
                        row[OutKey] = value.Insert(_startIndex, _value);
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}