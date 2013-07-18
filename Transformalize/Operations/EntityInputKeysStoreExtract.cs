using System.Collections.Generic;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {
    public class EntityInputKeysStoreExtract : AbstractOperation {
        private readonly Entity _entity;

        public EntityInputKeysStoreExtract(Entity entity) {
            _entity = entity;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            return _entity.InputKeys;
        }
    }
}