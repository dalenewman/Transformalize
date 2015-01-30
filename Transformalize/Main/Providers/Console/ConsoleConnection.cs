using Transformalize.Configuration;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Operations;
using Transformalize.Operations.Transform;

namespace Transformalize.Main.Providers.Console {
    public class ConsoleConnection : AbstractConnection {

        public override int NextBatchId(string processName) {
            return 1;
        }

        public override void WriteEndVersion(Process process, AbstractConnection input, Entity entity, bool force = false) {
            //nope  
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
            return new ConsoleLoadOperation(entity);
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

        public ConsoleConnection(TflConnection element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            Type = ProviderType.Console;
        }
    }
}