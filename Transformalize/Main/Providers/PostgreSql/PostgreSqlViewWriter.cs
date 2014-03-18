using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main.Providers.PostgreSql
{
    public class PostgreSqlViewWriter : WithLoggingMixin, IViewWriter {

        public void Create(Process process) {
            Warn("PostgreSql View Creator is not implemented yet.");
        }

        public void Drop(Process process) {
            Warn("PostgreSql View Dropper is not implemented yet.");
        }
    }
}