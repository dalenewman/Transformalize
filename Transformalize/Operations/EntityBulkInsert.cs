using System.Data.SqlClient;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations
{
    public class EntityBulkInsert : SqlBulkInsertOperation {
        private readonly Entity _entity;

        public EntityBulkInsert(Entity entity) : base(entity.OutputConnection.ConnectionString, entity.OutputName()) {
            _entity = entity;
            UseTransaction = false;
            TurnOptionOn(SqlBulkCopyOptions.TableLock);
        }

        protected override void PrepareSchema() {
            var fields = new FieldSqlWriter(_entity.All).ExpandXml().Output().AddBatchId(false).Context();
            foreach (var pair in fields) {
                Schema[pair.Key] = pair.Value.SystemType;
            }
        }
    }
}