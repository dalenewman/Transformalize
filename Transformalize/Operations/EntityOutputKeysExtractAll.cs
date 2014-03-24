using System.Collections.Generic;
using System.Data;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations
{
    public class EntityOutputKeysExtractAll : InputCommandOperation {
        private readonly Process _process;
        private readonly Entity _entity;
        private readonly List<string> _fields;

        public EntityOutputKeysExtractAll(Process process, Entity entity)
            : base(process.OutputConnection) {
            _process = process;
            _entity = entity;
            _fields = new List<string>(new FieldSqlWriter(entity.PrimaryKey).Alias(process.OutputConnection.L, process.OutputConnection.R).Keys()) { "TflKey" };
            }

        protected override Row CreateRowFromReader(IDataReader reader) {
            var row = new Row();
            foreach (var field in _fields) {
                row[field] = reader[field];
            }
            return row;

        }

        protected override void PrepareCommand(IDbCommand cmd) {
            cmd.CommandTimeout = 0;
            cmd.CommandText = PrepareSql();
            Debug("SQL:\r\n{0}", cmd.CommandText);
        }

        private string PrepareSql() {
            var connection = _process.OutputConnection;
            
            var sqlPattern = "SELECT {0}, TflKey FROM {1}";
            if (connection.NoLock) {
                sqlPattern += " WITH (NOLOCK);";
            } else {
                sqlPattern = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " + sqlPattern + "; COMMIT;";
            }

            var selectKeys = new FieldSqlWriter(_entity.PrimaryKey).Alias(connection.L, connection.R).Write(", ", false);
            return string.Format(sqlPattern, selectKeys, connection.Enclose(_entity.OutputName()));
        }

    }
}