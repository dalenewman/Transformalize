using Transformalize.Libs.Rhino.Etl;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.MySql
{
    public class MySqlTflWriter : WithLoggingMixin, ITflWriter {

        public void Initialize(Process process) {
            TflLogger.Warn(process.Name, string.Empty, "MySql Output Initialize isn't available yet.");
        }
    }
}