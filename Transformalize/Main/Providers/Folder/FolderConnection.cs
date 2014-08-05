using Transformalize.Configuration;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main.Providers.File;
using Transformalize.Operations.Transform;

namespace Transformalize.Main.Providers.Folder {

    public class FolderConnection : AbstractConnection {

        public override int NextBatchId(string processName) {
            return 1;
        }

        public override void WriteEndVersion(AbstractConnection input, Entity entity, bool force = false) {
            //nope  
        }

        public override IOperation EntityOutputKeysExtract(Entity entity) {
            return new EmptyOperation();
        }

        public override IOperation EntityOutputKeysExtractAll(Entity entity) {
            return new EmptyOperation();
        }

        public override IOperation EntityBulkLoad(Entity entity) {
            return new EmptyOperation();
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

        public override Fields GetEntitySchema(Process process, string name, string schema = "", bool isMaster = false) {
            var file = Folder.TrimEnd("\\".ToCharArray()) + "\\" + name.TrimStart("\\".ToCharArray());
            return new FieldInspector().Inspect(file);
        }

        public FolderConnection(ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            Type = ProviderType.Folder;
        }

    }
}