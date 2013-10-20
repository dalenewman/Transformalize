using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class FormatOperation : AbstractOperation {
        private readonly string _outKey;
        private readonly string _format;
        private readonly IEnumerable<KeyValuePair<string, IParameter>> _parameters;

        public FormatOperation(string outKey, string format, IParameters parameters) {
            _outKey = outKey;
            _format = format;
            _parameters = parameters.ToEnumerable();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                var linqRow = row;
                row[_outKey] = string.Format(_format, _parameters.Select(p => linqRow[p.Key] ?? p.Value).ToArray());
                yield return row;
            }
        }
    }
}