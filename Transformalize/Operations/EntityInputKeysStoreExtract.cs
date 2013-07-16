using System.Collections.Generic;
using System.Linq;
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
            if (!_entity.EntityVersionReader.HasRows)
                yield break;
            if (_entity.EntityVersionReader.IsRange && _entity.EntityVersionReader.BeginAndEndAreEqual())
                yield break;

            foreach (var row in _entity.InputKeys) {
                yield return row;
            }
        }
    }
}