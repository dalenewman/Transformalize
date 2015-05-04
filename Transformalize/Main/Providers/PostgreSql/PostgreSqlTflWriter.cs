namespace Transformalize.Main.Providers.PostgreSql
{
    public class PostgreSqlTflWriter : ITflWriter {

        public void Initialize(Process process) {
            process.Logger.Warn("PostgreSql Output Initialize isn't available yet.");
        }
    }
}