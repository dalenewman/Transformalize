using System.Collections.Generic;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations
{
    public class EntityBatchId : AbstractOperation {
        private readonly Entity _entity;

        public EntityBatchId(Entity entity) {
            _entity = entity;
            UseTransaction = false;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                row["TflId"] = _entity.TflId;
                yield return row;
            }
        }
    }
}