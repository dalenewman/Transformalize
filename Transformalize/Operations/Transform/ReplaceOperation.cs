using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class ReplaceOperation : ShouldRunOperation {
        private readonly string _oldValue;
        private readonly string _newValue;

        public ReplaceOperation(string inKey, string outKey, string oldValue, string newValue)
            : base(inKey, outKey) {
            _oldValue = oldValue;
            _newValue = newValue;
            Name = string.Format("ReplaceOperation ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = row[InKey].ToString().Replace(_oldValue, _newValue);
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}