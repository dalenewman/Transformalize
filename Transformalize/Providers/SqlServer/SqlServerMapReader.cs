using System.Data.SqlClient;
using Transformalize.Core;
using Transformalize.Core.Parameters_;

namespace Transformalize.Providers.SqlServer
{
    public class SqlServerMapReader : IMapReader {
        private readonly string _sql;
        private readonly string _connectionString;

        public SqlServerMapReader(string sql, string connectionString) {
            _sql = sql;
            _connectionString = connectionString;
        }

        public Map Read() {
            var map = new Map();

            using (var cn = new SqlConnection(_connectionString)) {
                cn.Open();
                var cmd = new SqlCommand(_sql, cn);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows) {
                    while (reader.Read()) {
                        map[reader.GetValue(0).ToString()] = new Item(reader.GetValue(1));
                    }
                }
            }

            return map;
        }
    }
}