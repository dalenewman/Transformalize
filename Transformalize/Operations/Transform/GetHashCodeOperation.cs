using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class GetHashCodeOperation : AbstractOperation {
        private readonly string _inKey;
        private readonly string _outKey;

        public GetHashCodeOperation(string inKey, string outKey) {
            _inKey = inKey;
            _outKey = outKey;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                row[_outKey] = row[_inKey].GetHashCode();
                yield return row;
            }
        }
    }
}