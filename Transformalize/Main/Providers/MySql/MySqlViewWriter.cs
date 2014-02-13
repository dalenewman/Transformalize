using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main.Providers.MySql
{
    public class MySqlViewWriter : WithLoggingMixin, IViewWriter {

        public void Create(Process process) {
            Warn("MySql View Creator is not implemented yet.");
        }

        public void Drop(Process process) {
            Warn("MySql View Dropper is not implemented yet.");
        }
    }

}