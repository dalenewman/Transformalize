using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Libs.Sqloogle.Operations.Support {

    public class UnionOperation : AbstractAggregationOperation {

        private readonly IEnumerable<string> _groupByColumns;

        public UnionOperation(IEnumerable<string> groupByColumns) {
            _groupByColumns = groupByColumns;
        }

        protected override void Accumulate(Row row, Row aggregate) {

            foreach (var column in _groupByColumns) {
                aggregate[column] = row[column];
            }

            if (aggregate["Count"] == null)
                aggregate["Count"] = 0;

            aggregate["Count"] = (int)aggregate["Count"] + 1;
        }

        protected override string[] GetColumnsToGroupBy() {
            return _groupByColumns.ToArray();
        }
    }
}
