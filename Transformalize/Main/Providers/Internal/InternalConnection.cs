using Transformalize.Configuration;

namespace Transformalize.Main.Providers.Internal
{
    public class InternalConnection : AbstractConnection {
        public InternalConnection(ConnectionConfigurationElement element, AbstractProvider provider, IConnectionChecker connectionChecker, IScriptRunner scriptRunner, IProviderSupportsModifier providerSupportsModifier, IEntityRecordsExist recordsExist, IEntityDropper dropper, ITflWriter tflWriter, IViewWriter viewWriter)
            : base(element, provider, connectionChecker, scriptRunner, providerSupportsModifier, recordsExist, dropper, tflWriter, viewWriter) { }

        public override string UserProperty { get { return string.Empty; } }
        public override string PasswordProperty { get { return string.Empty; } }
        public override string PortProperty { get { return string.Empty; } }
        public override string DatabaseProperty { get { return string.Empty; } }
        public override string ServerProperty { get { return string.Empty; } }
        public override string TrustedProperty { get { return string.Empty; } }

        public InternalConnection(Process process, ConnectionConfigurationElement element, AbstractProvider provider, IConnectionChecker connectionChecker, IScriptRunner scriptRunner, IProviderSupportsModifier providerSupportsModifier, IEntityRecordsExist recordsExist, IEntityDropper dropper, ITflWriter tflWriter, IViewWriter viewWriter)
            : base(element, provider, connectionChecker, scriptRunner, providerSupportsModifier, recordsExist, dropper, tflWriter, viewWriter) {

            TypeAndAssemblyName = process.Providers[element.Provider.ToLower()];

            EntityKeysQueryWriter = new EmptyQueryWriter();
            EntityKeysRangeQueryWriter = new EmptyQueryWriter();
            EntityKeysAllQueryWriter = new EmptyQueryWriter();
            TableQueryWriter = new EmptyTableQueryWriter();
            }
    }
}