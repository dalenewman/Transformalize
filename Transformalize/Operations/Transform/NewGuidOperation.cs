using System;
using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {

    public class NewGuidOperation : ShouldRunOperation {
        public NewGuidOperation(string inKey, string outKey)
            : base(inKey, outKey) {
            Name = string.Format("NewGuid ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = Guid.NewGuid();
                }
                yield return row;
            }
        }
    }
}