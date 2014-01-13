using System.Collections.Generic;
using Transformalize.Extensions;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class LeftOperation : TflOperation {
        private readonly int _length;

        public LeftOperation(string inKey, string outKey, int length)
            : base(inKey, outKey) {
            _length = length;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {

                    var value = row[InKey].ToString();
                    if (value.Length > _length)
                        row[OutKey] = row[InKey].ToString().Left(_length);

                }
                yield return row;
            }
        }
    }
}