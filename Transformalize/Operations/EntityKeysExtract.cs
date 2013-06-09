using System.Data;
using System.Data.SqlClient;
using Transformalize.Model;
using Transformalize.Readers;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations
{
    public class EntityKeysExtract : AbstractSqlInputOperation {
        private readonly Entity _entity;

        public EntityKeysExtract(Entity entity)
            : base(entity.InputConnection.ConnectionString) {
            _entity = entity;
            }

        protected override Row CreateRowFromReader(IDataReader reader) {
            return Row.FromReader(reader);
        }

        protected override void PrepareCommand(IDbCommand cmd) {

            var versionReader = new VersionReader(_entity);
            var begin = versionReader.GetBeginVersion();
            var end = versionReader.GetEndVersion();

            if (!versionReader.HasRows) {
                Warn("The entity is empty!");
            }

            cmd.CommandText = _entity.EntitySqlWriter.SelectKeys(versionReader.IsRange);

            if (versionReader.IsRange)
                cmd.Parameters.Add(new SqlParameter("@Begin", begin));
            cmd.Parameters.Add(new SqlParameter("@End", end));

        }
    }
}