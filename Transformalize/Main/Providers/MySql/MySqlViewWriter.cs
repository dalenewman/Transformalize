namespace Transformalize.Main.Providers.MySql
{
    public class MySqlViewWriter : IViewWriter {

        public void Create(Process process) {
            process.Logger.Warn("MySql View Creator is not implemented yet.");
        }

        public void Drop(Process process) {
            process.Logger.Warn( "MySql View Dropper is not implemented yet.");
        }
    }

}