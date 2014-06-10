using System.IO;

namespace Transformalize.Main.Providers.SqlCe
{
    public class SqlCeConnectionChecker : DefaultConnectionChecker, IConnectionChecker {

        public new bool Check(AbstractConnection connection) {
            if (CachedResults.ContainsKey(connection.Name)) {
                return CachedResults[connection.Name];
            }

            if (!new FileInfo(connection.Server).Exists) {
                Log.Warn("{0} not found.", connection.Server);

                var type = System.Type.GetType("System.Data.SqlServerCe.SqlCeEngine, System.Data.SqlServerCe", false, true);
                dynamic engine = System.Activator.CreateInstance(type, connection.GetConnectionString());
                engine.CreateDatabase();
                
                Log.Warn("Created {0} database file.", connection.Server);
            };

            return CheckConnection(connection);
        }
    }
}