using System.Data.SqlClient;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Data {
    public class SqlServerEntityDropper : WithLoggingMixin, IEntityDropper {

        private const string FORMAT = "DROP TABLE [{0}].[{1}];";

        public void DropOutput(Entity entity) {
            if (!new SqlServerEntityExists().OutputExists(entity)) return;

            using (var cn = new SqlConnection(entity.OutputConnection.ConnectionString)) {
                cn.Open();
                new SqlCommand(string.Format(FORMAT, entity.Schema, entity.OutputName()), cn).ExecuteNonQuery();
                Info("{0} | Dropped {1}.{2}", entity.ProcessName, entity.Schema, entity.OutputName());
            }
        }
    }
}