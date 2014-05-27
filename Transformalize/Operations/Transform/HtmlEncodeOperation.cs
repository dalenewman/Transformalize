using System.Collections.Generic;
using System.Web;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class HtmlEncodeOperation : ShouldRunOperation {
        public HtmlEncodeOperation(string inKey, string outKey)
            : base(inKey, outKey) {
            Name = "HtmlEncode (" + outKey + ")";
        }
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = HttpUtility.HtmlEncode(row[InKey]);
                }
                yield return row;
            }
        }
    }
}