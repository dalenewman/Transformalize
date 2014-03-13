namespace Transformalize.Main.Providers.Log {
    public class MailProvider : AbstractProvider {
        public MailProvider() {
            Type = ProviderType.Mail;
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