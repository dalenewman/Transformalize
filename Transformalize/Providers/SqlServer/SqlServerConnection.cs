using Transformalize.Configuration;
using Transformalize.Core.Process_;

namespace Transformalize.Providers.SqlServer
{
    public class SqlServerConnection : AbstractConnection
    {
        public SqlServerConnection(Process process, ConnectionConfigurationElement element, AbstractProvider provider, AbstractConnectionChecker connectionChecker, IScriptRunner scriptRunner, IProviderSupportsModifier providerScriptModifer)
            : base(element, provider, connectionChecker, scriptRunner, providerScriptModifer)
        {
            TypeAndAssemblyName = process.Providers[element.Provider.ToLower()];

            EntityKeysQueryWriter = process.Options.Top > 0 ? (IEntityQueryWriter)new SqlServerEntityKeysTopQueryWriter(process.Options.Top) : new SqlServerEntityKeysQueryWriter();
            EntityKeysRangeQueryWriter = new SqlServerEntityKeysRangeQueryWriter();
            EntityKeysAllQueryWriter = new SqlServerEntityKeysAllQueryWriter();
            TableQueryWriter = new SqlServerTableQueryWriter();
        }
    }
}