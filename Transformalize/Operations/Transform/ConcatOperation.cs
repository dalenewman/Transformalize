using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class ConcatOperation : TflOperation {
        private readonly IEnumerable<KeyValuePair<string, IParameter>> _parameters;

        public ConcatOperation(string outKey, IParameters parameters)
            : base(string.Empty, outKey) {
            _parameters = parameters.ToEnumerable();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var linqRow = row;
                    row[OutKey] = string.Concat(_parameters.Select(p => (linqRow[p.Key] ?? p.Value.Value).ToString()));
                }
                yield return row;
            }
        }
    }
}