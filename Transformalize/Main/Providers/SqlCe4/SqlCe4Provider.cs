namespace Transformalize.Main.Providers.SqlCe4 {
    public class SqlCe4Provider : AbstractProvider {
        public SqlCe4Provider() {
            IsDatabase = true;
            Type = ProviderType.SqlServerCe;
            L = "[";
            R = "]";
            Supports = new ProviderSupports {
                InsertMultipleRows = false,
                MaxDop = false,
                NoCount = false,
                NoLock = false,
                Top = false,
                TableVariable = false,
                ConnectionTimeout = false,
                IndexInclude = false,
                Views = false,
                Schemas = false
            };
        }
    }
}