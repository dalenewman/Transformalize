using System.Data.SqlClient;
using Transformalize.Model;

namespace Transformalize.Data {
    public class SqlServerEntityCounter : IEntityCounter {
        private readonly IConnectionChecker _connectionChecker;
        private readonly SqlServerEntityExists _entityExists;

        public SqlServerEntityCounter(IConnectionChecker connectionChecker = null) {
            _connectionChecker = connectionChecker;
            _entityExists = new SqlServerEntityExists();
        }

        public int CountInput(Entity entity) {
            if (_connectionChecker == null || _connectionChecker.Check(entity.InputConnection.ConnectionString)) {
                if (_entityExists.InputExists(entity)) {
                    using (var cn = new SqlConnection(entity.InputConnection.ConnectionString)) {
                        cn.Open();
                        var sql = string.Format("SELECT COUNT(*) FROM [{0}].[{1}] WITH (NOLOCK);", entity.Schema, entity.Name);
                        var cmd = new SqlCommand(sql, cn);
                        return (int)cmd.ExecuteScalar();
                    }
                }

            }
            return 0;
        }

        public int CountOutput(Entity entity) {
            if (_connectionChecker == null || _connectionChecker.Check(entity.OutputConnection.ConnectionString)) {
                if (_entityExists.OutputExists(entity)) {
                    using (var cn = new SqlConnection(entity.OutputConnection.ConnectionString)) {
                        cn.Open();
                        var sql = string.Format("SELECT COUNT(*) FROM [dbo].[{0}] WITH (NOLOCK);", entity.OutputName());
                        var cmd = new SqlCommand(sql, cn);
                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            return 0;
        }
    }
}