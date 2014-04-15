using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class FormatOperation : ShouldRunOperation {

        private readonly string _format;
        private readonly IEnumerable<KeyValuePair<string, IParameter>> _parameters;

        public FormatOperation(string outKey, string format, IParameters parameters)
            : base(string.Empty, outKey) {
            _format = format;
            _parameters = parameters.ToEnumerable();
            Name = string.Format("FormatOperation ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var linqRow = row;
                    row[OutKey] = string.Format(_format, _parameters.Select(p => linqRow[p.Key] ?? p.Value).ToArray());
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}