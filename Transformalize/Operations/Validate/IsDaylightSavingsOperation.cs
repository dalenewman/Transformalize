using System;
using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Operations.Transform;

namespace Transformalize.Operations.Validate {
    public class IsDaylightSavingsOperation : ShouldRunOperation {
        public IsDaylightSavingsOperation(string inKey, string outKey)
            : base(inKey, outKey) {
            Name = "IsDaylingSavings (" + outKey + ")";
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = ((DateTime)row[InKey]).IsDaylightSavingTime();
                }
                yield return row;
            }
        }
    }
}