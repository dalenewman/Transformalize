using System.Data.SqlClient;
using Transformalize.Model;

namespace Transformalize.Data
{
    public class SqlServerEntityExists : IEntityExists {

        private const string FORMAT = @"
IF EXISTS(
	SELECT *
	FROM INFORMATION_SCHEMA.TABLES 
	WHERE TABLE_SCHEMA = '{0}' 
	AND  TABLE_NAME = '{1}'
)	SELECT 1
ELSE
	SELECT 0;";

        public bool OutputExists(Entity entity) {
            using (var cn = new SqlConnection(entity.OutputConnection.ConnectionString)) {
                cn.Open();
                var sql = string.Format(FORMAT, entity.Schema, entity.OutputName());
                var cmd = new SqlCommand(sql, cn);
                return (int)cmd.ExecuteScalar() == 1;
            }
        }

        public bool InputExists(Entity entity) {
            using (var cn = new SqlConnection(entity.InputConnection.ConnectionString)) {
                cn.Open();
                var sql = string.Format(FORMAT, entity.Schema, entity.Name);
                var cmd = new SqlCommand(sql, cn);
                return (int)cmd.ExecuteScalar() == 1;
            }
        }
    }
}