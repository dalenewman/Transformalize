using System.Data.SqlClient;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations {
    public class EntityDelete : SqlBatchOperation {
        private readonly string _name;
        private readonly bool _isMaster;

        public EntityDelete(Process process, Entity entity)
            : base(process.OutputConnection) {
            _name = process.OutputConnection.Enclose(entity.OutputName());
            _isMaster = entity.IsMaster();
            BatchSize = process.OutputConnection.BatchSize;
            UseTransaction = true;
        }

        protected override void PrepareCommand(Row row, SqlCommand command) {
            if (_isMaster) {
                command.CommandText = string.Format("UPDATE {0} SET TflDeleted = @TflDeleted WHERE TflKey = @TflKey;", _name);
                AddParameter(command, "@TflDeleted", true);
            } else {
                command.CommandText = string.Format("DELETE FROM {0} WHERE TflKey = @TflKey;", _name);
            }
            AddParameter(command, "@TflKey", row["TflKey"]);
        }
    }
}