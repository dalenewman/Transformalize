using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations {
    public class CollectorOperation : AbstractOperation {
        private IEnumerable<Row> _rows = new List<Row>();

        public IEnumerable<Row> Rows {
            get { return _rows; }
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            _rows = rows;
            yield break;
        }
    }
}