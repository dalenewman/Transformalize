using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class PadLeftOperation : TflOperation {

        private readonly int _totalWidth;
        private readonly char _paddingChar;

        public PadLeftOperation(string inKey, string outKey, int totalWidth, string paddingChar)
            : base(inKey, outKey) {
            _totalWidth = totalWidth;
            _paddingChar = paddingChar[0];
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = row[InKey].ToString().PadLeft(_totalWidth, _paddingChar);
                }
                yield return row;
            }
        }
    }
}