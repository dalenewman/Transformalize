using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class LengthOperation : TflOperation {

        public LengthOperation(string inKey, string outKey)
            : base(inKey, outKey) {
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = row[InKey].ToString().Length;
                }
                yield return row;
            }
        }
    }
}