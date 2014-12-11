using Transformalize.Libs.Rhino.Etl;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.MySql
{
    public class MySqlViewWriter : WithLoggingMixin, IViewWriter {

        public void Create(Process process) {
            TflLogger.Warn(process.Name, string.Empty, "MySql View Creator is not implemented yet.");
        }

        public void Drop(Process process) {
            TflLogger.Warn(process.Name, string.Empty, "MySql View Dropper is not implemented yet.");
        }
    }

}