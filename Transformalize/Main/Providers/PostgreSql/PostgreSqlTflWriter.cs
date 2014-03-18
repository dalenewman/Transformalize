using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main.Providers.PostgreSql
{
    public class PostgreSqlTflWriter : WithLoggingMixin, ITflWriter {

        public void Initialize(Process process) {
            Warn("PostgreSql Output Initialize isn't available yet.");
        }
    }
}