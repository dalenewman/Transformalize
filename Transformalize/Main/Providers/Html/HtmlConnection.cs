using Transformalize.Configuration;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Operations;
using Transformalize.Operations.Load;
using Transformalize.Operations.Transform;

namespace Transformalize.Main.Providers.Html {

    public class HtmlConnection : AbstractConnection {

        public override int NextBatchId(string processName) {
            return 1;
        }

        public override void WriteEndVersion(Process process, AbstractConnection input, Entity entity, bool force = false) {
            //do nothing
        }

        public override IOperation ExtractCorrespondingKeysFromOutput(Entity entity) {
            return new EmptyOperation();
        }

        public override IOperation ExtractAllKeysFromOutput(Entity entity) {
            return new EmptyOperation();
        }

        public override IOperation ExtractAllKeysFromInput(Process process, Entity entity) {
            return new EmptyOperation();
        }

        public override IOperation Insert(Process process, Entity entity) {
            var pp = new PartialProcessOperation(process);
            pp.Register(new HtmlRowOperation(entity, "HtmlRow"));
            pp.RegisterLast(new HtmlLoadOperation(this, entity, "HtmlRow"));
            return pp;
        }

        public override IOperation Update(Entity entity) {
            return new EmptyOperation();
        }

        public override void LoadBeginVersion(Entity entity) {
            throw new System.NotImplementedException();
        }

        public override void LoadEndVersion(Entity entity) {
            throw new System.NotImplementedException();
        }

        public override Fields GetEntitySchema(Process process, Entity entity, bool isMaster = false) {
            return new Fields();
        }

        public override IOperation Delete(Entity entity) {
            throw new System.NotImplementedException();
        }

        public override IOperation Extract(Process process, Entity entity, bool firstRun) {
            throw new System.NotImplementedException();
        }

        public HtmlConnection(ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            Type = ProviderType.Html;
        }

    }
}