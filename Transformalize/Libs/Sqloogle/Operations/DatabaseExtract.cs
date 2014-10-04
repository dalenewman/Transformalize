using System.Data;
using System.Data.SqlClient;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main.Providers;

namespace Transformalize.Libs.Sqloogle.Operations {
    /// <summary>
    /// This is SQLoogle's (SQL Server) Database Extract operation. 
    /// It implements the PrepareCommand and CreateRowFromReader methods.  
    /// It creates a database specific connection string for each database.
    /// </summary>
    public class DatabaseExtract : InputCommandOperation {
        private readonly AbstractConnection _connection;

        public DatabaseExtract(AbstractConnection connection) : base(connection)
        {
            _connection = connection;
        }

        protected override void PrepareCommand(IDbCommand cmd) {
            cmd.CommandText = @"/* SQLoogle */
                USE master;

                SELECT
                    database_id AS databaseid
                    ,[Name] AS [database]
                    ,Compatibility_Level AS compatibilitylevel
                FROM sys.databases WITH (NOLOCK)
                WHERE [state] = 0
                AND [user_access] = 0
                AND [is_in_standby] = 0
                AND compatibility_level >= 80
                ORDER BY [name] ASC;
            ";
        }

        protected override Row CreateRowFromReader(IDataReader reader) {
            var row = Row.FromReader(reader);
            row["connectionstring"] = GetDatabaseSpecificConnectionString(row);
            return row;
        }

        protected string GetDatabaseSpecificConnectionString(Row row) {
            var builder = new SqlConnectionStringBuilder(_connection.GetConnectionString()) {
                InitialCatalog = row["database"].ToString()
            };
            return builder.ConnectionString;
        }

    }
}
