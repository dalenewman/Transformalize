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


    public class JoinTransformOperation : ShouldRunOperation {
        private readonly string _separator;
        private readonly IEnumerable<KeyValuePair<string, IParameter>> _parameters;

        public JoinTransformOperation(string outKey, string separator, IParameters parameters)
            : base(string.Empty, outKey) {
            _separator = separator;
            _parameters = parameters.ToEnumerable();
            Name = string.Format("JoinOperation ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var linqRow = row;
                    row[OutKey] = string.Join(_separator, _parameters.Select(p => linqRow[p.Key] ?? p.Value).Where(p => !p.Equals(string.Empty)));
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}