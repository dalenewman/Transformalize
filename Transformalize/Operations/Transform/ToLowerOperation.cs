using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations.Transform
{
    public class ToLowerOperation : AbstractOperation {
        private readonly string _inKey;
        private readonly string _outKey;

        public ToLowerOperation(string inKey, string outKey)
        {
            _inKey = inKey;
            _outKey = outKey;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                row[_outKey] = row[_inKey].ToString().ToLower();
                yield return row;
            }
        }
    }
}