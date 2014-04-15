using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class LengthOperation : ShouldRunOperation {

        public LengthOperation(string inKey, string outKey)
            : base(inKey, outKey) {
            Name = string.Format("LengthOperation ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = row[InKey].ToString().Length;
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}