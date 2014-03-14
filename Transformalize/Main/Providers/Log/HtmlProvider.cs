namespace Transformalize.Main.Providers.Log {
    public class HtmlProvider : AbstractProvider {
        public HtmlProvider() {
            Type = ProviderType.Html;
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