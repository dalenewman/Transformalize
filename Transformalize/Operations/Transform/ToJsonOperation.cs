using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net.fastJSON;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Rhino.Etl;
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
                    row[OutKey] = JsonConvert.SerializeObject(data, Formatting.None);
                } else {
                    Skip();
                }

                yield return row;
            }
        }
    }
}