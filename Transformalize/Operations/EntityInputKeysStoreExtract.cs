using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;
using Transformalize.Model;

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