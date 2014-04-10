using System.Collections.Generic;
using System.Data;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Main.Providers.SqlServer
{
    public class SqlServerEntityOutputKeysExtractAll : InputCommandOperation {

        private readonly Entity _entity;
        private readonly List<string> _fields;

        public SqlServerEntityOutputKeysExtractAll(AbstractConnection connection, Entity entity)
            : base(connection) {
            _entity = entity;
            _fields = new List<string>(new FieldSqlWriter(entity.PrimaryKey).Alias(connection.L, connection.R).Keys()) { "TflKey" };
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
            
            var sqlPattern = "SELECT {0}, TflKey FROM {1}";
            if (Connection.NoLock) {
                sqlPattern += " WITH (NOLOCK);";
            } else {
                sqlPattern = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " + sqlPattern + "; COMMIT;";
            }

            var selectKeys = new FieldSqlWriter(_entity.PrimaryKey).Alias(Connection.L, Connection.R).Write(", ", false);
            return string.Format(sqlPattern, selectKeys, Connection.Enclose(_entity.OutputName()));
        }

    }
}