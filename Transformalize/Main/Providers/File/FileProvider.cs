namespace Transformalize.Main.Providers.File {
    public class FileProvider : AbstractProvider {
        public FileProvider() {
            Type = ProviderType.File;
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