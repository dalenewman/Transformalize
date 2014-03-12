namespace Transformalize.Main.Providers.Console {
    public class ConsoleProvider : AbstractProvider {
        public ConsoleProvider() {
            Type = ProviderType.Console;
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