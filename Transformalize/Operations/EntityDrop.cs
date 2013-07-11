using System.Collections.Generic;
using Transformalize.Data;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {
    public class EntityDrop : AbstractOperation {
        private readonly IEntityDropper _entityDropper;
        private readonly Entity _entity;

        public EntityDrop(Entity entity, IEntityDropper entityDropper = null) {
            _entity = entity;
            _entityDropper = entityDropper ?? new SqlServerEntityDropper();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            _entityDropper.DropOutput(_entity);
            return rows;
        }

    }
}