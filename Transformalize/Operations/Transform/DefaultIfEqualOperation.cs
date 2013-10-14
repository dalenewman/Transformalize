using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations.Transform
{
    public class DefaultIfEqualOperation : AbstractOperation {
        private readonly string _inKey;
        private readonly string _outKey;
        private readonly object _default;
        private readonly object _value;

        public DefaultIfEqualOperation(string inKey, string outKey, object @default, object value) {
            _inKey = inKey;
            _outKey = outKey;
            _default = @default;
            _value = value;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (row[_inKey].Equals(_value)) {
                    row[_outKey] = _default;
                }
                yield return row;
            }
        }
    }
}