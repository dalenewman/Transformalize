using System.IO;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.SqlCe {
    public class SqlCeConnectionChecker : DefaultConnectionChecker{

        public SqlCeConnectionChecker(ILogger logger, int timeOut = 3)
            : base(logger, timeOut) {
        }

        public new bool Check(AbstractConnection connection, ILogger logger) {
            if (CachedResults.ContainsKey(connection.Name)) {
                return CachedResults[connection.Name];
            }

            if (!new FileInfo(connection.Server).Exists) {
                logger.Warn("{0} not found.", connection.Server);

                var type = System.Type.GetType("System.Data.SqlServerCe.SqlCeEngine, System.Data.SqlServerCe", false, true);
                dynamic engine = System.Activator.CreateInstance(type, connection.GetConnectionString());
                engine.CreateDatabase();

                logger.Warn("Created {0} database file.", connection.Server);
            };

            return CheckConnection(connection);
        }
    }
}