using System.Collections.Generic;
using System.Data.SqlClient;

namespace Transformalize.Data
{
    public class SqlServerMapReader : IMapReader {
        private readonly string _sql;
        private readonly string _connectionString;

        public SqlServerMapReader(string sql, string connectionString) {
            _sql = sql;
            _connectionString = connectionString;
        }

        public Dictionary<string, object> Read() {
            var map = new Dictionary<string, object>();

            using (var cn = new SqlConnection(_connectionString)) {
                cn.Open();
                var cmd = new SqlCommand(_sql, cn);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows) {
                    while (reader.Read()) {
                        map[reader.GetValue(0).ToString()] = reader.GetValue(1);
                    }
                }
            }

            return map;
        }
    }
}