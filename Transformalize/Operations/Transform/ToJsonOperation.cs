using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.fastJSON;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class ToJsonOperation : TflOperation {
        private readonly IParameters _parameters;

        public ToJsonOperation(string outKey, IParameters parameters)
            : base(string.Empty, outKey) {
            _parameters = parameters;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var data = new Dictionary<string, object>();
                    foreach (var pair in _parameters) {
                        data[pair.Value.Name] = pair.Value.Value ?? row[pair.Key];
                    }
                    row[OutKey] = JSON.Instance.ToJSON(data);
                }
                yield return row;
            }
        }
    }
}