using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class ReplaceOperation : TflOperation {
        private readonly string _oldValue;
        private readonly string _newValue;

        public ReplaceOperation(string inKey, string outKey, string oldValue, string newValue)
            : base(inKey, outKey) {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = row[InKey].ToString().Replace(_oldValue, _newValue);
                }
                yield return row;
            }
        }
    }
}