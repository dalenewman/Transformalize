using System.Data;

namespace Transformalize.Providers.SqlServer
{
    public class SqlServerCompatibilityReader : ICompatibilityReader {

        public Compatibility Read(IConnection connection)
        {
            var compatibility = new Compatibility();

            using (var cn = connection.GetConnection())
            {
                cn.Open();
                var cmd = cn.CreateCommand();
                cmd.CommandText = "SELECT compatibility_level FROM sys.DATABASES WHERE [name] = @Database;";
                
                AddDatabaseParameter(connection, cmd);

                compatibility.CanInsertMultipleRows = (byte)cmd.ExecuteScalar() > 90;
                compatibility.SupportsLimit = false;
                compatibility.SupportsNoLock = true;
                compatibility.SupportsTop = true;
            }
            return compatibility;
        }

        private static void AddDatabaseParameter(IConnection connection, IDbCommand cmd)
        {
            var database = cmd.CreateParameter();
            database.ParameterName = "@Database";
            database.Value = connection.Database;
            cmd.Parameters.Add(database);
        }
    }
}