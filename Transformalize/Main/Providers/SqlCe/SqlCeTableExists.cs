using System.Data;

namespace Transformalize.Main.Providers.SqlCe
{
    public class SqlCeTableExists : ITableExists {

        private readonly AbstractConnection _connection;

        public SqlCeTableExists(AbstractConnection connection) {
            _connection = connection;
        }

        public bool OutputExists(string name) {
            var sql = string.Format(@"
	            SELECT TABLE_NAME
	            FROM INFORMATION_SCHEMA.TABLES 
	            WHERE TABLE_NAME = '{0}';", name)
                ;
            using (var cn = _connection.GetConnection()) {
                cn.Open();
                var cmd = cn.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                using (var reader = cmd.ExecuteReader()) {
                    return reader.Read();
                }
            }
        }
    }
}