using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Libs.Sqloogle.Operations.Support {

    public class AppendToRowOperation : AbstractOperation {

        private readonly IDictionary<string, object> _data;

        public AppendToRowOperation(string key, object value) {
            _data = new Dictionary<string, object> {{key, value}};
        }

        public AppendToRowOperation(IDictionary<string, object> data) {
            _data = data;
        }

        public AppendToRowOperation Add(KeyValuePair<string, object> data) {
            _data.Add(data);
            return this;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                foreach (var kv in _data) {
                    row.Add(kv.Key, kv.Value);
                }
                yield return row;
            }
        }
    }
}
