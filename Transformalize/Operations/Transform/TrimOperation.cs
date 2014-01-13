using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class TrimOperation : TflOperation {
        private readonly char[] _trimChars;

        public TrimOperation(string inKey, string outKey, string trimChars)
            : base(inKey, outKey) {
            _trimChars = trimChars.ToCharArray();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = row[InKey].ToString().Trim(_trimChars);
                }
                yield return row;
            }

        }
    }
}