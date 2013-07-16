using System.Collections.Generic;
using System.Linq;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {
    public class EntityInputKeysStore : AbstractOperation {
        private readonly Entity _entity;

        public EntityInputKeysStore(Entity entity) {
            _entity = entity;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            _entity.InputKeys = rows.ToList();
            yield break;
        }

    }
}