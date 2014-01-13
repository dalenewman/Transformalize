using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class JoinTransformOperation : TflOperation {
        private readonly string _separator;
        private readonly IEnumerable<KeyValuePair<string, IParameter>> _parameters;

        public JoinTransformOperation(string outKey, string separator, IParameters parameters)
            : base(string.Empty, outKey) {
            _separator = separator;
            _parameters = parameters.ToEnumerable();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var linqRow = row;
                    row[OutKey] = string.Join(_separator, _parameters.Select(p => linqRow[p.Key] ?? p.Value).Where(p => !p.Equals(string.Empty)));
                }
                yield return row;
            }
        }
    }
}