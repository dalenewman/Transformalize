namespace Transformalize.Main.Providers.MySql {

    public class MySqlTflWriter : ITflWriter {
        public void Initialize(Process process) {
            process.Logger.Warn("MySql Output Initialize isn't available yet.");
        }
    }
}