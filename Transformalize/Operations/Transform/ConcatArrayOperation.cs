using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Transformalize.Main.Parameters;

namespace Transformalize.Operations.Transform {
    public class ConcatArrayOperation : ShouldRunOperation {
        private readonly IParameter _inParameter;

        public ConcatArrayOperation(IParameter inParameter, string outKey)
            : base(inParameter.Name, outKey) {
            _inParameter = inParameter;
            Name = string.Format("ConcatArrayOperation ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var sb = StringBuilders.GetObject();
                    var array = row[_inParameter.Name] as object[];
                    if (array != null) {
                        for (var i = 0; i < array.Length; i++) {
                            sb.Append(array[i]);
                        }
                    } else {
                        sb.Append(row[_inParameter.Name] ?? (_inParameter.Value ?? string.Empty));
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