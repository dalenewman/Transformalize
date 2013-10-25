using System.Collections.Generic;
using System.Globalization;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations.Transform
{
    public class ToTitleCaseOperation : AbstractOperation {
        private readonly string _inKey;
        private readonly string _outKey;
        private readonly TextInfo _textInfo;

        public ToTitleCaseOperation(string inKey, string outKey) {
            _inKey = inKey;
            _outKey = outKey;
            _textInfo = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                row[_outKey] = _textInfo.ToTitleCase(row[_inKey].ToString().ToLower());
                yield return row;
            }
        }
    }
}