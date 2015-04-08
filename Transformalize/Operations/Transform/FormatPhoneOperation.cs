using System.Collections.Generic;
using System.Text.RegularExpressions;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class FormatPhoneOperation : ShouldRunOperation {
        private readonly Regex _clean = new Regex("[^0-9]", RegexOptions.Compiled);
        public FormatPhoneOperation(string inKey, string outKey)
            : base(inKey, outKey) {
            Name = string.Format("FormatPhone ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var clean = _clean.Replace(row[InKey].ToString(), string.Empty);
                    if (clean.Length == 10) {
                        row[OutKey] = string.Format("({0}) {1}-{2}", clean.Substring(0, 3), clean.Substring(3, 3),
                            clean.Substring(6, 4));
                    } else {
                        row[OutKey] = clean;
                    }
                } else {
                    Skip();
                }
                yield return row;
            }
        }
    }
}