using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Operations.Transform;

namespace Transformalize.Operations.Validate {
    public class IsEmptyOperation : ShouldRunOperation {
        public IsEmptyOperation(string inKey, string outKey)
            : base(inKey, outKey) {
            Name = "IsEmpty (" + InKey + " => " + OutKey + ")";
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = row[InKey].Equals(string.Empty);
                } else { Skip(); }
                yield return row;
            }
        }
    }
}