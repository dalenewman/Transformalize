using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class GetHashCodeOperation : ShouldRunOperation {

        public GetHashCodeOperation(string inKey, string outKey) : base(inKey, outKey)
        {
            Name = string.Format("GetHashCodeOperation ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = row[InKey].GetHashCode();
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}