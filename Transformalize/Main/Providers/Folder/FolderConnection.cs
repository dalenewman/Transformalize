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

        public override Fields GetEntitySchema(Process process, Entity entity, bool isMaster = false) {
            var file = Folder.TrimEnd("\\".ToCharArray()) + "\\" + entity.Name.TrimStart("\\".ToCharArray());
            return new FieldInspector(process.Logger).Inspect(file);
        }

        public override IOperation Delete(Entity entity) {
            throw new System.NotImplementedException();
        }

        public override IOperation Extract(Process process, Entity entity, bool firstRun) {
            var union = new SerialUnionAllOperation(entity);
            foreach (var file in new DirectoryInfo(Folder).GetFiles(SearchPattern, SearchOption)) {
                File = file.FullName;
                if (Is.Excel()) {
                    union.Add(new FileExcelExtract(this, entity));
                } else {
                    if (Is.Delimited()) {
                        union.Add(new FileDelimitedExtract(this, entity));
                    } else {
                        union.Add(new FileFixedExtract(this, entity));
                    }
                }
                union.Add();
            }
            return union;
        }

        public FolderConnection(TflConnection element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            Type = ProviderType.Folder;
        }

    }
}