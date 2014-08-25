using System.Collections.Generic;
using System.Data;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Main.Providers.Sql
{
    public class SqlEntityKeysExtractAllFromOutput : InputCommandOperation {

        private readonly Entity _entity;
        private readonly List<string> _keys;

        public SqlEntityKeysExtractAllFromOutput(AbstractConnection connection, Entity entity)
            : base(connection) {
            _entity = entity;
            _keys = new List<string>(entity.PrimaryKey.Aliases()) { "TflKey" };
            }

        protected override Row CreateRowFromReader(IDataReader reader) {
            var row = new Row();
            foreach (var key in _keys) {
                row[key] = reader[key];
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