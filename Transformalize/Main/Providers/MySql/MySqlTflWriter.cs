using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main.Providers.MySql
{
    public class MySqlTflWriter : WithLoggingMixin, ITflWriter {

        public void Initialize(Process process) {
            Warn("MySql Output Initialize isn't available yet.");
        }
    }
}