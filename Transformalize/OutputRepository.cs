using System.Data.SqlClient;
using Transformalize.Configuration;
using Dapper;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize {

    public class OutputRepository : WithLoggingMixin {

        private readonly ProcessConfiguration _process;

        public OutputRepository(ProcessConfiguration process) {
            _process = process;
        }

        private static void Execute(string sql, string connectionString) {
            using (var cn = new SqlConnection(connectionString)) {
                cn.Open();
                cn.Execute(sql);
            }
        }

        public void InitializeOutput() {
            Execute(_process.TruncateOutputSql(), _process.Connection.Value);
            Info("{0} | Truncated {1}", _process.Name, _process.Output);

            Execute(_process.DropOutputSql(), _process.Connection.Value);
            Info("{0} | Dropped {1}", _process.Name, _process.Output);
            
            Execute(_process.CreateSql(), _process.Connection.Value);
            Info("{0} | Created {1}", _process.Name, _process.Output);
        }

        public void InitializeEntityTracker() {
            Execute(_process.TruncateEntityTrackerSql(), _process.Connection.Value);
            Info("{0} | Truncated EntityTracker.", _process.Name);
            Execute(_process.DropEntityTrackerSql(), _process.Connection.Value);
            Info("{0} | Dropped EntityTracker.", _process.Name);
            Execute(_process.CreateEntityTrackerSql(), _process.Connection.Value);
            Info("{0} | Created EntityTracker.", _process.Name);
        }
    }
}
