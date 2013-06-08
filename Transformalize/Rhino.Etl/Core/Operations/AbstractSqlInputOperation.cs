using System.Configuration;

namespace Transformalize.Rhino.Etl.Core.Operations {

    /// <summary>
    /// Generic input command operation that takes the
    /// actual connection string itself, rather than a
    /// connection string from the configuration, or a
    /// connection string settings object.
    /// </summary>
    public abstract class AbstractSqlInputOperation : InputCommandOperation {

        private const string PROVIDER = "System.Data.SqlClient.SqlConnection, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        private static string _connectionString;

        protected AbstractSqlInputOperation(string connectionString)
            : base(GetConnectionStringSettings(connectionString)) {
            UseTransaction = false;
        }

        private static ConnectionStringSettings GetConnectionStringSettings(string connectionString) {
            _connectionString = connectionString;
            return new ConnectionStringSettings {
                ConnectionString = _connectionString,
                ProviderName = PROVIDER,
            };
        }

    }
}
