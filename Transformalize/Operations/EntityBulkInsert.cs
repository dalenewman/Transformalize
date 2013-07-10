using System.Data.SqlClient;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {
    public class EntityBulkInsert : SqlBulkInsertOperation {
        private readonly Entity _entity;
        private long _count;

        public EntityBulkInsert(Entity entity)
            : base(entity.OutputConnection.ConnectionString, entity.OutputName()) {
            _entity = entity;
            UseTransaction = false;
            TurnOptionOn(SqlBulkCopyOptions.TableLock);
            TurnOptionOff(SqlBulkCopyOptions.UseInternalTransaction);
        }

        protected override void PrepareSchema() {
            NotifyBatchSize = 1000;
            BatchSize = _entity.OutputConnection.OutputBatchSize;

            var fields = new FieldSqlWriter(_entity.All).ExpandXml().Output().AddBatchId(false).Context();
            foreach (var pair in fields) {
                Schema[pair.Key] = pair.Value.SystemType;
            }
        }

        protected override void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e) {
            _count = e.RowsCopied;
            Info("{0} | Processed {1} rows in EntityBulkInsert", _entity.ProcessName, _count);
        }

        public override void Dispose() {
            _entity.RecordsAffected = _count;
            base.Dispose();
        }
    }
}