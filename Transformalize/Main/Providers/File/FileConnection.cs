using Transformalize.Configuration;
using Transformalize.Libs.Ninject.Syntax;
using Transformalize.Main.Providers.Internal;

namespace Transformalize.Main.Providers.File {
    public class FileConnection : AbstractConnection {

        public override string UserProperty { get { return string.Empty; } }
        public override string PasswordProperty { get { return string.Empty; } }
        public override string PortProperty { get { return string.Empty; } }
        public override string DatabaseProperty { get { return string.Empty; } }
        public override string ServerProperty { get { return string.Empty; } }
        public override string TrustedProperty { get { return string.Empty; } }

        public FileConnection(Process process, ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies) : base(element, dependencies) {

            TypeAndAssemblyName = process.Providers[element.Provider.ToLower()];
            EntityKeysQueryWriter = new EmptyQueryWriter();
            EntityKeysRangeQueryWriter = new EmptyQueryWriter();
            EntityKeysAllQueryWriter = new EmptyQueryWriter();
        }
    }
}