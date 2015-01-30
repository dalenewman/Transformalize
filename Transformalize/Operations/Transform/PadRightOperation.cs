using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class PadRightOperation : ShouldRunOperation {
        private readonly int _totalWidth;
        private readonly char _paddingChar;

        public PadRightOperation(string inKey, string outKey, int totalWidth, char paddingChar)
            : base(inKey, outKey) {
            _totalWidth = totalWidth;
            _paddingChar = paddingChar;
            Name = string.Format("PadRightOperation ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = row[InKey].ToString().PadRight(_totalWidth, _paddingChar);
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}