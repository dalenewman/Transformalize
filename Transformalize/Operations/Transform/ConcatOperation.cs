using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations.Transform
{
    public class ConcatOperation : AbstractOperation {
        private readonly string _outKey;
        private readonly IEnumerable<KeyValuePair<string, IParameter>> _parameters;

        public ConcatOperation(string outKey, IParameters parameters) {
            _outKey = outKey;
            _parameters = parameters.ToEnumerable();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                var linqRow = row;
                row[_outKey] = string.Concat(_parameters.Select(p => linqRow[p.Key] ?? p.Value));
                yield return row;
            }
        }
    }
}