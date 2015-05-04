namespace Transformalize.Main.Providers.PostgreSql
{
    public class PostgreSqlViewWriter : IViewWriter {

        public void Create(Process process) {
           process.Logger.Warn("PostgreSql View Creator is not implemented yet.");
        }

        public void Drop(Process process) {
            process.Logger.Warn("PostgreSql View Dropper is not implemented yet.");
        }
    }
}