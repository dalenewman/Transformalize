namespace Transformalize.Providers.MySql
{
    public class MySqlProvider : AbstractProvider
    {
        public MySqlProvider()
        {
            Type = ProviderType.MySql;
            L = "`";
            R = "`";
            Supports = new ProviderSupports()
                           {
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