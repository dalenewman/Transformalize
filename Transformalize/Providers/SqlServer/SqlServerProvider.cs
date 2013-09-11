namespace Transformalize.Providers.SqlServer
{
    public class SqlServerProvider : AbstractProvider
    {
        public SqlServerProvider()
        {
            Type = ProviderType.SqlServer;
            L = "[";
            R = "]";
            Supports = new ProviderSupports()
                           {
                               InsertMultipleRows = true,
                               MaxDop = true,
                               NoCount = true,
                               NoLock = true,
                               Top = true,
                               TableVariable = true
                           };
        }
    }
}