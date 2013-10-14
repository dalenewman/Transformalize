using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Libs.fastJSON;
using Transformalize.Main;

namespace Transformalize.Operations.Transform
{
    public class ToJsonOperation : AbstractOperation {
        private readonly string _outKey;
        private readonly IParameters _parameters;

        public ToJsonOperation(string outKey, IParameters parameters) {
            _outKey = outKey;
            _parameters = parameters;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                var data = new Dictionary<string, object>();
                foreach (var pair in _parameters) {
                    data[pair.Value.Name] = pair.Value.Value ?? row[pair.Key];
                }

                row[_outKey] = JSON.Instance.ToJSON(data);
                yield return row;
            }
        }
    }
}