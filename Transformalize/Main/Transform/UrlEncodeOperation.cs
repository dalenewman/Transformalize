using System.Collections.Generic;
using System.Web;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Operations.Transform;

namespace Transformalize.Main {
    public class UrlEncodeOperation : ShouldRunOperation {
        public UrlEncodeOperation(string inKey, string outKey)
            : base(inKey, outKey) {
            Name = "UrlEncode (" + OutKey + ")";
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = HttpUtility.UrlEncode(row[InKey].ToString());
                }
                yield return row;
            }
        }
    }
}