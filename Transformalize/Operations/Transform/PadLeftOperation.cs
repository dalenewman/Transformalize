using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations.Transform
{
    public class PadLeftOperation : AbstractOperation {
        private readonly string _inKey;
        private readonly string _outKey;
        private readonly int _totalWidth;
        private readonly char _paddingChar;

        public PadLeftOperation(string inKey, string outKey, int totalWidth, string paddingChar) {
            _inKey = inKey;
            _outKey = outKey;
            _totalWidth = totalWidth;
            _paddingChar = paddingChar[0];
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                row[_outKey] = row[_inKey].ToString().PadLeft(_totalWidth, _paddingChar);
                yield return row;
            }
        }
    }
}