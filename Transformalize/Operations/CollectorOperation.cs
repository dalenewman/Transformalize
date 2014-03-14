using System.Collections.Generic;
using Transformalize.Libs.NLog.Internal;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations {
    public class CollectorOperation : AbstractOperation {

        public IList<Row> Rows { get; private set; }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            Rows = rows.ToList();
            yield break;
        }
    }
}