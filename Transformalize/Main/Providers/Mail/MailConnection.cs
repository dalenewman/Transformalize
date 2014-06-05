using Transformalize.Configuration;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Operations.Load;
using Transformalize.Operations.Transform;

namespace Transformalize.Main.Providers.Mail {
    public class MailConnection : AbstractConnection {

        public override string UserProperty { get { return string.Empty; } }
        public override string PasswordProperty { get { return string.Empty; } }
        public override string PortProperty { get { return string.Empty; } }
        public override string DatabaseProperty { get { return string.Empty; } }
        public override string ServerProperty { get { return string.Empty; } }
        public override string TrustedProperty { get { return string.Empty; } }
        public override string PersistSecurityInfoProperty { get { return string.Empty; } }

        public override int NextBatchId(string processName) {
            return 1;
        }

        public override void WriteEndVersion(AbstractConnection input, Entity entity, bool force = false) {
            // do nothing
        }

        public override IOperation EntityOutputKeysExtract(Entity entity) {
            return new EmptyOperation();
        }

        public override IOperation EntityOutputKeysExtractAll(Entity entity) {
            return new EmptyOperation();
        }

        public override IOperation EntityBulkLoad(Entity entity) {
            return new MailLoadOperation(entity);
        }

        public override IOperation EntityBatchUpdate(Entity entity) {
            return new EmptyOperation();
        }

        public override void LoadBeginVersion(Entity entity) {
            throw new System.NotImplementedException();
        }

        public override void LoadEndVersion(Entity entity) {
            throw new System.NotImplementedException();
        }

        public override EntitySchema GetEntitySchema(string name, string schema = "", bool isMaster = false) {
            return new EntitySchema();
        }

        public MailConnection(ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            Type = ProviderType.Mail;
        }

    }
}