using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {

    public class EntityToOutput : AbstractOperation {
        private readonly Entity _entity;

        public EntityToOutput(Entity entity) {
            _entity = entity;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            using (var cn = new SqlConnection(_entity.OutputConnection.ConnectionString)) {
                cn.Open();
                foreach (var group in rows.Partition(_entity.InputConnection.BatchUpdateSize)) {
                    var g = group.ToArray();
                    var cmd = new SqlCommand(_entity.EntitySqlWriter.UpsertSql(g), cn);
                    Trace(cmd.CommandText);
                    cmd.ExecuteNonQuery();
                    foreach (var row in g) {
                        yield return row;
                    }
                }
            }
        }

    }
}