using System.Collections.Generic;
using Transformalize.Data;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {
    public class EntityBatchId : AbstractOperation {
        private readonly Entity _entity;

        public EntityBatchId(Entity entity, IEntityBatch entityBatch) {
            _entity = entity;
            _entity.TflBatchId = entityBatch.GetNext(_entity);
            UseTransaction = false;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                row["TflBatchId"] = _entity.TflBatchId;
                yield return row;
            }
        }

        public override void Dispose() {
            _entity.RecordsAffected = (int)Statistics.OutputtedRows;
            base.Dispose();
        }
    }
}