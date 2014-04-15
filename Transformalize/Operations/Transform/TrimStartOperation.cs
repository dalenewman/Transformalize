using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class TrimStartOperation : ShouldRunOperation {
        private readonly char[] _trimChars;

        public TrimStartOperation(string inKey, string outKey, string trimChars)
            : base(inKey, outKey) {
            _trimChars = trimChars.ToCharArray();
            Name = string.Format("TrimStartOperation ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = row[InKey].ToString().TrimStart(_trimChars);
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}