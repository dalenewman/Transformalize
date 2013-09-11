using Transformalize.Configuration;
using Transformalize.Core.Process_;

namespace Transformalize.Providers.MySql
{
    public class MySqlConnection : AbstractConnection
    {
        public MySqlConnection(Process process, ConnectionConfigurationElement element, AbstractProvider provider, AbstractConnectionChecker connectionChecker, IScriptRunner scriptRunner, IProviderSupportsModifier providerScriptModifer)
            : base(element, provider, connectionChecker, scriptRunner, providerScriptModifer)
        {
            TypeAndAssemblyName = process.Providers[element.Provider.ToLower()];

            EntityKeysQueryWriter = process.Options.Top > 0 ? (IEntityQueryWriter)new MySqlEntityKeysTopQueryWriter(process.Options.Top) : new MySqlEntityKeysQueryWriter();
            EntityKeysRangeQueryWriter = new MySqlEntityKeysRangeQueryWriter();
            EntityKeysAllQueryWriter = new MySqlEntityKeysAllQueryWriter();
            TableQueryWriter = new MySqlTableQueryWriter();
        }
    }
}