namespace Transformalize.Main.Providers.Folder {
    public class FolderProvider : AbstractProvider {
        public FolderProvider() {
            Type = ProviderType.Folder;
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