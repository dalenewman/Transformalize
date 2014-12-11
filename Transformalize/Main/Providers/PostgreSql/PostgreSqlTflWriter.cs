using Transformalize.Libs.Rhino.Etl;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.PostgreSql
{
    public class PostgreSqlTflWriter : WithLoggingMixin, ITflWriter {

        public void Initialize(Process process) {
            TflLogger.Warn(process.Name, string.Empty, "PostgreSql Output Initialize isn't available yet.");
        }
    }
}