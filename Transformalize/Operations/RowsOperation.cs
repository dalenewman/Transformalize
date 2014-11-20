using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations {
    public class RowsOperation : AbstractOperation {
        private readonly List<Row> _rows;

        public RowsOperation(IEnumerable<Row> rows) {
            _rows = rows.ToList();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            return _rows;
        }
    }
}