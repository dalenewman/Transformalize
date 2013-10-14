using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations.Transform
{
    public class ReplaceOperation : AbstractOperation {
        private readonly string _inKey;
        private readonly string _outKey;
        private readonly string _oldValue;
        private readonly string _newValue;

        public ReplaceOperation(string inKey, string outKey, string oldValue, string newValue) {
            _inKey = inKey;
            _outKey = outKey;
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                row[_outKey] = row[_inKey].ToString().Replace(_oldValue, _newValue);
                yield return row;
            }
        }
    }
}