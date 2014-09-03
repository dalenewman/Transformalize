using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.UnidecodeSharpFork;

namespace Transformalize.Operations.Transform {
    public class TransliterateOperation : ShouldRunOperation {
        public TransliterateOperation(string inKey, string outKey)
            : base(inKey, outKey) {
            Name = "Transliterate (" + outKey + ")";
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = row[InKey].ToString().Unidecode();
                }
                yield return row;
            }
        }
    }
}