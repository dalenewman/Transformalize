using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class TrimStartAppendOperation : ShouldRunOperation {

        private readonly string _separator;
        private readonly char[] _trimChars;

        public TrimStartAppendOperation(string inKey, string outKey, string trimChars, string separator)
            : base(inKey, outKey) {
            _separator = separator;
            _trimChars = trimChars.ToCharArray();
            Name = string.Format("TrimStartAppendOperation ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var orginal = row[InKey].ToString();
                    row[OutKey] = string.Join(_separator, new[] { orginal, orginal.TrimStart(_trimChars) });
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}