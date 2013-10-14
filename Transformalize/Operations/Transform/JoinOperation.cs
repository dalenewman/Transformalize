using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations.Transform
{
    public class JoinOperation : AbstractOperation {
        private readonly string _outKey;
        private readonly string _separator;
        private readonly IEnumerable<KeyValuePair<string, IParameter>> _parameters;

        public JoinOperation(string outKey, string separator, IParameters parameters) {
            _outKey = outKey;
            _separator = separator;
            _parameters = parameters.ToEnumerable();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                var linqRow = row;
                row[_outKey] = string.Join(_separator, _parameters.Select(p => linqRow[p.Key] ?? p.Value));
                yield return row;
            }
        }
    }
}