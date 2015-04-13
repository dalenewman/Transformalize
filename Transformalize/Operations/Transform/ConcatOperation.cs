using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Transformalize.Main.Parameters;

namespace Transformalize.Operations.Transform {
    public class ConcatOperation : ShouldRunOperation {

        private readonly KeyValuePair<string, IParameter>[] _parameters;

        public ConcatOperation(string outKey, IParameters parameters)
            : base(string.Empty, outKey) {
            _parameters = parameters.ToEnumerable().ToArray();
            Name = string.Format("ConcatOperation ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var sb = StringBuilders.GetObject();
                    for (var i = 0; i < _parameters.Count(); i++) {
                        sb.Append(row[_parameters[i].Key] ?? _parameters[i].Value.Value);
                    }
                    row[OutKey] = sb.ToString();
                    sb.Clear();
                    StringBuilders.PutObject(sb);
                } else {
                    Skip();
                }
                yield return row;
            }
        }
    }
}