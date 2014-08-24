using System.IO;
using Transformalize.Configuration;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main.Providers.File;
using Transformalize.Operations.Extract;
using Transformalize.Operations.Transform;

namespace Transformalize.Main.Providers.Folder {

    public class FolderConnection : AbstractConnection {

        public override int NextBatchId(string processName) {
            return 1;
        }

        public override void WriteEndVersion(AbstractConnection input, Entity entity, bool force = false) {
            //nope  
        }

        public override IOperation ExtractCorrespondingKeysFromOutput(Entity entity) {
            return new EmptyOperation();
        }

        public override IOperation ExtractAllKeysFromOutput(Entity entity) {
            return new EmptyOperation();
        }

        public override IOperation ExtractAllKeysFromInput(Entity entity) {
            return new EmptyOperation();
        }

        public override IOperation Insert(Entity entity) {
            return new EmptyOperation();
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

        public override Fields GetEntitySchema(Process process, string name, string schema = "", bool isMaster = false) {
            var file = Folder.TrimEnd("\\".ToCharArray()) + "\\" + name.TrimStart("\\".ToCharArray());
            return new FieldInspector().Inspect(file);
        }

        public override IOperation Delete(Entity entity) {
            throw new System.NotImplementedException();
        }

        public override IOperation Extract(Entity entity, bool firstRun) {
            var union = new SerialUnionAllOperation();
            foreach (var file in new DirectoryInfo(Folder).GetFiles(SearchPattern, SearchOption)) {
                File = file.FullName;
                if (Is.Excel()) {
                    union.Add(new FileExcelExtract(entity, this, entity.Top));
                } else {
                    if (Is.Delimited()) {
                        union.Add(new FileDelimitedExtract(this, entity, entity.Top));
                    } else {
                        union.Add(new FileFixedExtract(this, entity, entity.Top));
                    }
                }

                union.Add();
            }
            return union;

        }

        public FolderConnection(ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            Type = ProviderType.Folder;
        }

    }
}