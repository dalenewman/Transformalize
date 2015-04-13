using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main.Parameters;

namespace Transformalize.Operations.Transform {
    public class FormatArrayOperation : ShouldRunOperation {

        private readonly string _format;

        public FormatArrayOperation(IParameter inParameter, string outKey, string format)
            : base(inParameter.Name, outKey) {
            _format = format;
            Name = string.Format("FormatArrayOperation ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var array = row[InKey] as object[];
                    if (array == null) {
                        row[OutKey] = string.Format(_format, row[InKey]);
                    } else {
                        row[OutKey] = string.Format(_format, array);
                    }
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}