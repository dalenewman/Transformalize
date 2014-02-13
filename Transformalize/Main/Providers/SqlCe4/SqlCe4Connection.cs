using Transformalize.Configuration;
using Transformalize.Main.Providers.Internal;
using Transformalize.Main.Providers.SqlServer;

namespace Transformalize.Main.Providers.SqlCe4 {
    public class SqlCe4Connection : AbstractConnection {

        public override string UserProperty { get { return string.Empty; } }
        public override string PasswordProperty { get { return string.Empty; } }
        public override string PortProperty { get { return string.Empty; } }
        public override string DatabaseProperty { get { return string.Empty; } }
        public override string ServerProperty { get { return "Data Source"; } }
        public override string TrustedProperty { get { return string.Empty; } }
        public override string PersistSecurityInfoProperty {
            get { return "Persist Security Info"; }
        }

        public SqlCe4Connection(Process process, ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {

            TypeAndAssemblyName = process.Providers[element.Provider.ToLower()];
            EntityKeysQueryWriter = process.Options.Top > 0 ? (IEntityQueryWriter)new SqlServerEntityKeysTopQueryWriter(process.Options.Top) : new SqlServerEntityKeysQueryWriter();
            EntityKeysRangeQueryWriter = new SqlServerEntityKeysRangeQueryWriter();
            EntityKeysAllQueryWriter = new SqlServerEntityKeysAllQueryWriter();
        }
    }
}