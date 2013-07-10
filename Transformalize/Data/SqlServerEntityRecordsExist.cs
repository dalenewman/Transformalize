using System.Data.SqlClient;
using Transformalize.Model;

namespace Transformalize.Data {
    public class SqlServerEntityRecordsExist : IEntityRecordsExist {
        private readonly SqlServerEntityExists _entityExists;

        private const string FORMAT = @"
IF EXISTS(
	SELECT *
	FROM [{0}].[{1}]
)	SELECT 1
ELSE
	SELECT 0;";

        public SqlServerEntityRecordsExist() {
            _entityExists = new SqlServerEntityExists();
        }

        public bool OutputRecordsExist(Entity entity) {
            if (_entityExists.OutputExists(entity)) {
                using (var cn = new SqlConnection(entity.OutputConnection.ConnectionString)) {
                    cn.Open();
                    var sql = string.Format(FORMAT, entity.Schema, entity.OutputName());
                    var cmd = new SqlCommand(sql, cn);
                    return (int)cmd.ExecuteScalar() == 1;
                }
            }
            return false;
        }

        public bool InputRecordsExist(Entity entity) {
            if (_entityExists.InputExists(entity)) {
                using (var cn = new SqlConnection(entity.InputConnection.ConnectionString)) {
                    cn.Open();
                    var sql = string.Format(FORMAT, entity.Schema, entity.Name);
                    var cmd = new SqlCommand(sql, cn);
                    return (int)cmd.ExecuteScalar() == 1;
                }
            }
            return false;
        }
    }
}