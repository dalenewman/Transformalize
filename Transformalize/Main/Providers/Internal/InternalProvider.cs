namespace Transformalize.Main.Providers.Internal {
    public class InternalProvider : AbstractProvider {
        public InternalProvider() {
            Type = ProviderType.Internal;
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