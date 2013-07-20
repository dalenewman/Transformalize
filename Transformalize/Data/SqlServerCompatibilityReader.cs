using System.Data.SqlClient;

namespace Transformalize.Data
{
    public class SqlServerCompatibilityReader : ICompatibilityReader {

        private readonly byte _compatibilityLevel;

        public SqlServerCompatibilityReader(IConnection connection) {
            
            using (var cn = new SqlConnection(connection.ConnectionString)) {
                cn.Open();
                const string sql = "SELECT compatibility_level FROM sys.DATABASES WHERE [name] = @Database;";
                var cmd = new SqlCommand(sql, cn);
                cmd.Parameters.Add(new SqlParameter("@Database", connection.Database));
                _compatibilityLevel = (byte) cmd.ExecuteScalar();
            }

            InsertMultipleValues = _compatibilityLevel > 90;
        }

        public bool InsertMultipleValues { get; private set; }
    }
}