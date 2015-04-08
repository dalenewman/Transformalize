using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations {
    /// <summary>
    /// Because the input and the output are the same, gather data before writing.
    /// </summary>
    public class GatherOperation : AbstractOperation {

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            return rows.ToArray();
        }
    }
}