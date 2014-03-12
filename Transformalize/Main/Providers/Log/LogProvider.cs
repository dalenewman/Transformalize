namespace Transformalize.Main.Providers.Log {
    public class LogProvider : AbstractProvider {
        public LogProvider() {
            Type = ProviderType.Log;
            L = string.Empty;
            R = string.Empty;
            Supports = new ProviderSupports {
                InsertMultipleRows = false,
                MaxDop = false,
                NoCount = false,
                NoLock = false,
                TableVariable = false,
                Top = false
            };
        }
    }
}