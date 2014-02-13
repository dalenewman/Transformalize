using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class TrimEndOperation : ShouldRunOperation {

        private readonly char[] _trimCharArray;

        public TrimEndOperation(string inKey, string outKey, string trimChars)
            : base(inKey, outKey) {
            _trimCharArray = trimChars.ToCharArray();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = row[InKey].ToString().TrimEnd(_trimCharArray);
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}