using System.Data.SqlServerCe;
using System.IO;

namespace Transformalize.Main.Providers.SqlCe4
{
    public class SqlCe4ConnectionChecker : DefaultConnectionChecker, IConnectionChecker {

        public new bool Check(AbstractConnection connection) {
            if (CachedResults.ContainsKey(connection.Name)) {
                return CachedResults[connection.Name];
            }

            if (!new FileInfo(connection.Server).Exists) {
                Log.Warn("{0} not found.", connection.Server);
                new SqlCeEngine(connection.GetConnectionString()).CreateDatabase();
                Log.Warn("Created {0} database file.", connection.Server);
            };

            return CheckConnection(connection);
        }
    }
}