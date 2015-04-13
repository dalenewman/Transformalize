using System.Collections.Generic;
using System.Threading;
using Transformalize.Extensions;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main.Parameters;

namespace Transformalize.Operations.Transform {
    public class JoinArrayOperation : ShouldRunOperation {
        private readonly IParameter _inParameter;
        private readonly string _separator;

        public JoinArrayOperation(IParameter inParameter, string outKey, string separator)
            : base(inParameter.Name, outKey) {
            _inParameter = inParameter;
            _separator = separator;
            Name = string.Format("JoinArrayOperation ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var sb = StringBuilders.GetObject();
                    var array = row[_inParameter.Name] as object[];
                    if (array != null) {
                        for (var i = 0; i < array.Length; i++) {
                            sb.Append(array[i]);
                            sb.Append(_separator);
                        }
                    } else {
                        sb.Append(row[_inParameter.Name] ?? (_inParameter.Value ?? string.Empty));
                        sb.Append(_separator);
                    }
                    sb.TrimEnd(_separator);
                    row[OutKey] = sb.ToString();
                    sb.Clear();
                    StringBuilders.PutObject(sb);
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}