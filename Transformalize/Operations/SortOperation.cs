using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations
{
    public class SortOperation : AbstractOperation {

        private readonly List<Sort> _orderBy = new List<Sort>();

        public SortOperation(Entity entity) {
            _orderBy.AddRange(entity.Fields.Sorts());
            _orderBy.AddRange(entity.CalculatedFields.Sorts());
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            var ordered = _orderBy[0].Order.StartsWith("a") ? rows.OrderBy(r => r[_orderBy[0].Field]) : rows.OrderByDescending(r => r[_orderBy[0].Field]);
            for (var i = 1; i < _orderBy.Count; i++) {
                var sort = _orderBy[i];
                ordered = sort.Order.StartsWith("a") ? ordered.ThenBy(r => r[sort.Field]) : ordered.ThenByDescending(r => r[sort.Field]);
            }
            return ordered;
        }
    }
}