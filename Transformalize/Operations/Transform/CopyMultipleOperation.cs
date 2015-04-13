using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Transformalize.Main.Parameters;

namespace Transformalize.Operations.Transform {
    public class CopyMultipleOperation : ShouldRunOperation {

        private readonly IEnumerable<KeyValuePair<string, IParameter>> _parameters;

        public CopyMultipleOperation(string outKey, IParameters parameters)
            : base(string.Empty, outKey) {
            _parameters = parameters.ToEnumerable();
            Name = string.Format("CopyMultipleOperation ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var linqRow = row;
                    row[OutKey] = _parameters.Select(p => linqRow[p.Key] ?? p.Value).ToArray();
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}