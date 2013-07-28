using System.Collections.Generic;
using System.Linq;

namespace Transformalize.Libs.Rhino.Etl.Core.Operations {
    
    public class DistinctOperation : AbstractAggregationOperation {

        private readonly IEnumerable<string> _columnsToGroupBy;

        public DistinctOperation(IEnumerable<string> columnsToGroupBy) {
            _columnsToGroupBy = columnsToGroupBy;
        }

        protected override void Accumulate(Row row, Row aggregate) {
            foreach (var column in _columnsToGroupBy) {
                aggregate[column] = row[column];
            }
        }

        protected override string[] GetColumnsToGroupBy() {
            return _columnsToGroupBy.ToArray();
        }
    }
}