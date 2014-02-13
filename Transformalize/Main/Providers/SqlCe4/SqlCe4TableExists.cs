using System.Data;

namespace Transformalize.Main.Providers.SqlCe4
{
    public class SqlCe4TableExists : ITableExists {

        private readonly AbstractConnection _connection;

        public SqlCe4TableExists(AbstractConnection connection) {
            _connection = connection;
        }

        public bool Exists(string schema, string name) {
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