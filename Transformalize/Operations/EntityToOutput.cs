using System.Collections.Generic;
using System.Data.SqlClient;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations
{
    public class EntityToOutput : AbstractOperation {
        private readonly Entity _entity;

        public EntityToOutput(Entity entity) {
            _entity = entity;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            using (var cn = new SqlConnection(_entity.OutputConnection.ConnectionString)) {
                cn.Open();
                foreach (var group in rows.Partition(_entity.InputConnection.BatchUpdateSize)) {
                    var cmd = new SqlCommand(_entity.EntitySqlWriter.UpsertSql(@group), cn);
                    Trace(cmd.CommandText);
                    cmd.ExecuteNonQuery();
                }
            }
            yield break;
        }

    }
}