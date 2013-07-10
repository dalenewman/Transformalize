using System.Data;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations
{
    public class EntityOutputKeysExtract : InputCommandOperation {

        const string SQL_PATTERN = @"SELECT {0}, TflKey FROM [{1}].[{2}] WITH (NOLOCK) ORDER BY {3};";
        private readonly Entity _entity;
        private readonly string _sql;

        public EntityOutputKeysExtract(Entity entity)
            : base(entity.OutputConnection.ConnectionString) {
            _entity = entity;
            _sql = PrepareSql();
            }

        protected override Row CreateRowFromReader(IDataReader reader) {
            return Row.FromReader(reader);
        }

        protected override void PrepareCommand(IDbCommand cmd) {
            cmd.CommandTimeout = 0;
            cmd.CommandText = _sql;
        }

        private string PrepareSql() {
            var sqlWriter = new FieldSqlWriter(_entity.PrimaryKey).Alias();
            var selectKeys = sqlWriter.Write(", ", false);
            var orderByKeys = sqlWriter.Asc().Write();
            return string.Format(SQL_PATTERN, selectKeys, _entity.Schema, _entity.OutputName(), orderByKeys);
        }
    }
}