using Transformalize.Configuration;

namespace Transformalize.Main.Providers.PostgreSql
{
    public class PostgreSqlConnection : AbstractConnection {

        /* Pooling=true;Min Pool Size=0;Max Pool Size=100;Connection Lifetime=0; */
        public override string UserProperty { get { return "User ID"; } }
        public override string PasswordProperty { get { return "Password"; } }
        public override string PortProperty { get { return "Port"; } }
        public override string DatabaseProperty { get { return "Database"; } }
        public override string ServerProperty { get { return "Host"; } }
        public override string TrustedProperty { get { return string.Empty; } }
        public override string PersistSecurityInfoProperty {
            get { return string.Empty; }
        }

        public PostgreSqlConnection(Process process, ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {

            TypeAndAssemblyName = process.Providers[element.Provider.ToLower()];
            EntityKeysQueryWriter = process.Options.Top > 0 ? (IEntityQueryWriter)new PostgreSqlEntityKeysTopQueryWriter(process.Options.Top) : new PostgreSqlEntityKeysQueryWriter();
            EntityKeysRangeQueryWriter = new PostgreSqlEntityKeysRangeQueryWriter();
            EntityKeysAllQueryWriter = new PostgreSqlEntityKeysAllQueryWriter();

            }
    }
}