using System.Data.SqlClient;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Main.Providers.Sql {
    public class SqlEntityDelete : SqlBatchOperation {
        private readonly string _name;
        private readonly bool _isMaster;

        public SqlEntityDelete(AbstractConnection connection, Entity entity)
            : base(connection) {
            _name = Connection.Enclose(entity.OutputName());
            _isMaster = entity.IsMaster();
            BatchSize = connection.BatchSize;
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