using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class ElipseOperation : ShouldRunOperation {
        private readonly int _length;
        private readonly string _elipse;

        public ElipseOperation(string inKey, string outKey, int length, string elipse)
            : base(inKey, outKey) {
            _length = length;
            _elipse = elipse;
            Name = string.Format("Elipse ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var value = row[InKey].ToString();
                    if (value.Length > _length) {
                        row[OutKey] = value.Substring(0, _length) + _elipse;
                    }
                } else {
                    Skip();
                }
                yield return row;
            }
        }
    }
}