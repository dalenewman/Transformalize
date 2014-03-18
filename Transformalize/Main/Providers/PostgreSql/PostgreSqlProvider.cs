namespace Transformalize.Main.Providers.PostgreSql {
    public class PostgreSqlProvider : AbstractProvider {
        public PostgreSqlProvider() {
            IsDatabase = true;
            Type = ProviderType.PostgreSql;
            L = "\"";
            R = "\"";
            Supports = new ProviderSupports {
                InsertMultipleRows = true,
                MaxDop = false,
                NoCount = false,
                NoLock = false,
                TableVariable = false,
                Top = false,
                ConnectionTimeout = false,
                Schemas = true
            };
        }
    }
}