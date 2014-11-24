using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations {
    public class RowsOperation : AbstractOperation {
        private readonly IEnumerable<Row> _rows;

        public RowsOperation(IEnumerable<Row> rows) {
            _rows = rows;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            return _rows;
        }
    }
}