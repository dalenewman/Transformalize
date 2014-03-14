using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations.Transform {
    public class FilterOutputOperation : AbstractOperation {
        private readonly Func<Row, bool> _shouldRun;

        public FilterOutputOperation(Func<Row, bool> shouldRun) {
            _shouldRun = shouldRun;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            return rows.Where(row => _shouldRun(row));
        }
    }
}