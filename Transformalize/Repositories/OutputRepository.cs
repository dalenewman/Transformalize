using System.Data.SqlClient;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Repositories {

    public class OutputRepository : WithLoggingMixin {

        private readonly Process _process;

        public OutputRepository(Process process) {
            _process = process;
        }

        private static void Execute(string sql, string connectionString) {
            using (var cn = new SqlConnection(connectionString)) {
                cn.Open();
                var command = new SqlCommand(sql, cn);
                command.ExecuteNonQuery();
            }
        }

        public void InitializeOutput() {
            Execute(SqlTemplates.TruncateTable(_process.Output), _process.OutputConnection.ConnectionString);
            Info("{0} | Truncated {1}", _process.Name, _process.Output);

            Execute(SqlTemplates.DropTable(_process.Output), _process.OutputConnection.ConnectionString);
            Info("{0} | Dropped {1}", _process.Name, _process.Output);
            
            Execute(_process.CreateOutputSql(), _process.OutputConnection.ConnectionString);
            Info("{0} | Created {1}", _process.Name, _process.Output);
        }

    }
}
