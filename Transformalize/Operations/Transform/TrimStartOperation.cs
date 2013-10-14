using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations.Transform
{
    public class TrimStartOperation : AbstractOperation {
        private readonly string _inKey;
        private readonly string _outKey;
        private readonly char[] _trimChars;

        public TrimStartOperation(string inKey, string outKey, string trimChars) {
            _inKey = inKey;
            _outKey = outKey;
            _trimChars = trimChars.ToCharArray();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                row[_outKey] = row[_inKey].ToString().TrimStart(_trimChars);
                yield return row;
            }
        }
    }
}