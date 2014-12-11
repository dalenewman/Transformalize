using Transformalize.Libs.Rhino.Etl;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.PostgreSql
{
    public class PostgreSqlViewWriter : WithLoggingMixin, IViewWriter {

        public void Create(Process process) {
            TflLogger.Warn(process.Name, string.Empty, "PostgreSql View Creator is not implemented yet.");
        }

        public void Drop(Process process) {
            TflLogger.Warn(process.Name, string.Empty, "PostgreSql View Dropper is not implemented yet.");
        }
    }
}