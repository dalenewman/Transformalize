using System.Collections.Generic;
using Transformalize.Libs.MarkdownSharp;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {

    public class MarkDownOperation : ShouldRunOperation {

        private readonly Markdown _markdown = new Markdown();
        public MarkDownOperation(string inKey, string outKey)
            : base(inKey, outKey) {
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = _markdown.Transform(row[InKey].ToString());
                } else {
                    Skip();
                }
                yield return row;
            }
        }
    }
}