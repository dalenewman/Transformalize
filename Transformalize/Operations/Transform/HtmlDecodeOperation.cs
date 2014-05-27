using System.Collections.Generic;
using System.Web;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class HtmlDecodeOperation : ShouldRunOperation {
        public HtmlDecodeOperation(string inKey, string outKey)
            : base(inKey, outKey) {
            Name = "HtmlDecode (" + outKey + ")";
        }
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = HttpUtility.HtmlDecode(row[InKey].ToString());
                }
                yield return row;
            }
        }
    }
}