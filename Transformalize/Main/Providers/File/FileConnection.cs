using System;
using Transformalize.Configuration;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Operations.Extract;
using Transformalize.Operations.Load;
using Transformalize.Operations.Transform;

namespace Transformalize.Main.Providers.File {

    public class FileConnection : AbstractConnection {

        public override int NextBatchId(string processName) {
            return 1;
        }

        public override void WriteEndVersion(Process process, AbstractConnection input, Entity entity, bool force = false) {
            //do nothing
        }

        public override IOperation ExtractCorrespondingKeysFromOutput(Entity entity) {
            var p = new Process("blank");
            return Extract(p, entity, true);
        }

        public override IOperation ExtractAllKeysFromOutput(Entity entity) {
            var p = new Process("blank");
            return Extract(p, entity, true);
        }

        public override IOperation ExtractAllKeysFromInput(Process process, Entity entity) {
            var p = new Process("blank");
            return Extract(p, entity, true);
        }

        public override IOperation Insert(Process process, Entity entity) {
            return new FileLoadOperation(this, entity);
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
            return new FieldInspector().Inspect(FileInformationFactory.Create(File), process.FileInspectionRequest);
        }

        public override IOperation Delete(Entity entity) {
            throw new NotImplementedException();
        }

        public override IOperation Extract(Process process, Entity entity, bool firstRun) {
            if (Direct) {
                return new FileContentsExtract(this, entity);
            }

            if (Is.Excel()) {
                return new FileExcelExtract(this, entity);
            }
            if (Is.Delimited()) {
                return new FileDelimitedExtract(this, entity);
            }
            return new FileFixedExtract(this, entity);
        }

        public FileConnection(TflConnection element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            Type = ProviderType.File;
        }
    }
}