using System;
using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class UtcNowOperation : ShouldRunOperation {
        public UtcNowOperation(string inKey, string outKey)
            : base(inKey, outKey) {
            Name = "UtcNow(" + inKey + "=>" + outKey + ")";
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                row[OutKey] = DateTime.UtcNow;
                yield return row;
            }
        }
    }
}