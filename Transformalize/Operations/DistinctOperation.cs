using System.Collections.Generic;
using System.Linq;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {
    
    public class DistinctOperation : AbstractAggregationOperation {

        private readonly IEnumerable<string> _columnsToGroupBy;

        public DistinctOperation(IEnumerable<string> columnsToGroupBy) {
            _columnsToGroupBy = columnsToGroupBy;
        }

        protected override void Accumulate(Row row, Row aggregate) {
            foreach (var column in _columnsToGroupBy.Where(column => aggregate[column] == null)) {
                aggregate[column] = row[column];
            }
        }

        protected override string[] GetColumnsToGroupBy() {
            return _columnsToGroupBy.ToArray();
        }
    }
}