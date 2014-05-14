using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Operations.Transform;

namespace Transformalize.Main
{
    public class AppendOperation : ShouldRunOperation {
        private readonly string _value;

        public AppendOperation(string inKey, string outKey, string value)
            : base(inKey, outKey) {
            _value = value;
            }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = row[InKey] + _value;
                }
                yield return row;
            }
        }
    }
}