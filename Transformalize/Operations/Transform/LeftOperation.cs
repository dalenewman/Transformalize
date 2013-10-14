using System.Collections.Generic;
using Transformalize.Extensions;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations.Transform {
    public class LeftOperation : AbstractOperation {
        private readonly string _inKey;
        private readonly string _outKey;
        private readonly int _length;

        public LeftOperation(string inKey, string outKey, int length) {
            _inKey = inKey;
            _outKey = outKey;
            _length = length;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                var value = row[_inKey].ToString();
                if (value.Length > _length)
                    row[_outKey] = row[_inKey].ToString().Left(_length);
                yield return row;
            }
        }
    }
}