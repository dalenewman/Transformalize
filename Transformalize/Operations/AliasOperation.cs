using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations
{
    public class AliasOperation : AbstractOperation {
        private readonly IEnumerable<Field> _fields;

        public AliasOperation(Entity entity) {
            _fields = entity.Fields.OrderBy(f => f.Value.Index).Select(f => f.Value);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                foreach (var field in _fields) {
                    row[field.Alias] = row[field.Name];
                }
                yield return row;
            }
        }
    }
}