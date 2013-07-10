using System.Collections.Generic;
using System.Linq;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {
    public class EntityActionFilter : AbstractOperation {
        private readonly Entity _entity;
        private readonly EntityAction _entityAction;

        public EntityActionFilter(ref Entity entity, EntityAction entityAction) {
            _entity = entity;
            _entityAction = entityAction;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            OnFinishedProcessing += EntityActionFilter_OnFinishedProcessing;
            return rows.Where(r => r["a"].Equals(_entityAction));
        }

        void EntityActionFilter_OnFinishedProcessing(IOperation obj) {
            _entity.RecordsAffected += obj.Statistics.OutputtedRows;
        }

    }
}