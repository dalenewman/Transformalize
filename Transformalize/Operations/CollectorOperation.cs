using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations {
    public class CollectorOperation : AbstractOperation {

        public Row[] Rows { get; private set; }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            Rows = rows.ToArray();
            yield break;
        }
    }
}