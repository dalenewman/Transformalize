using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.fastJSON;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class ToJsonOperation : ShouldRunOperation {
        private readonly IParameters _parameters;

        public ToJsonOperation(string outKey, IParameters parameters)
            : base(string.Empty, outKey) {
            _parameters = parameters;
            Name = string.Format("ToJsonOperation ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var data = new Dictionary<string, object>();
                    foreach (var pair in _parameters) {
                        data[pair.Value.Name] = pair.Value.Value ?? row[pair.Key];
                    }
                    row[OutKey] = JSON.Instance.ToJSON(data, new JSONParameters() { UseEscapedUnicode = false });
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}