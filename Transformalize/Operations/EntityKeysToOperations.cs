using System.Collections.Generic;
using System.Linq;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.ConventionOperations;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {

    public class EntityKeysToOperations : AbstractOperation {
        private readonly Entity _entity;
        private readonly string _operationColumn;

        public EntityKeysToOperations(Entity entity, string operationColumn = "operation") {
            _entity = entity;
            _operationColumn = operationColumn;
        }
        
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            return rows.Partition(_entity.InputConnection.InputBatchSize).Select(batch => new Row {{
                _operationColumn,
                new ConventionSqlInputOperation(_entity.InputConnection.ConnectionString) {
                    Sql = _entity.EntitySqlWriter.SelectByKeys(batch)
                }
            }});
        }
    }
}