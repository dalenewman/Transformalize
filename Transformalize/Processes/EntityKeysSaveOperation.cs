using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Processes {
    public class EntityKeysSaveOperation : AbstractOperation {
        private readonly Entity _entity;

        public EntityKeysSaveOperation(Entity entity) {
            _entity = entity;
            EntityName = entity.Name;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            var gathered = rows.ToArray();
            _entity.InputKeys = gathered.Select(r => r.Clone()).ToArray();
            return gathered;
        }
    }
}