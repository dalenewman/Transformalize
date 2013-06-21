using System.Collections.Generic;
using System.Data.SqlClient;
using Transformalize.Model;
using Transformalize.Readers;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {

    public class EntityKeysExtract : AbstractOperation {
        private readonly Entity _entity;

        public EntityKeysExtract(Entity entity) {
            _entity = entity;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            var versionReader = new VersionReader(_entity);
            var begin = versionReader.GetBeginVersion();
            _entity.End = versionReader.GetEndVersion();

            if (!versionReader.HasRows) {
                Warn("The entity is empty!");
            }

            using (var cn = new SqlConnection(_entity.InputConnection.ConnectionString)) {
                cn.Open();
                var cmd = new SqlCommand(_entity.EntitySqlWriter.SelectKeys(versionReader.IsRange), cn);

                if (versionReader.IsRange)
                    cmd.Parameters.Add(new SqlParameter("@Begin", begin));
                cmd.Parameters.Add(new SqlParameter("@End", _entity.End));

                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        yield return Row.FromReader(reader);
                    }
                }

            }

        }
    }
}