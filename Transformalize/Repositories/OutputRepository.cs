using System.Data.SqlClient;
using Dapper;
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
                cn.Execute(sql);
            }
        }

        public void InitializeOutput() {
            Execute(_process.TruncateOutputSql(), _process.OutputConnection.ConnectionString);
            Info("{0} | Truncated {1}", _process.Name, _process.Output);

            Execute(_process.DropOutputSql(), _process.OutputConnection.ConnectionString);
            Info("{0} | Dropped {1}", _process.Name, _process.Output);
            
            Execute(_process.CreateOutputSql(), _process.OutputConnection.ConnectionString);
            Info("{0} | Created {1}", _process.Name, _process.Output);
        }

    }
}
