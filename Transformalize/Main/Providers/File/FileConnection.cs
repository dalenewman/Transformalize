using System;
using System.IO;
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

        public override void WriteEndVersion(AbstractConnection input, Entity entity, bool force = false) {
            //do nothing
        }

        public override IOperation ExtractCorrespondingKeysFromOutput(Entity entity) {
            return Extract(entity, true);
        }

        public override IOperation ExtractAllKeysFromOutput(Entity entity) {
            return Extract(entity, true);
        }

        public override IOperation ExtractAllKeysFromInput(Entity entity) {
            return Extract(entity, true);
        }

        public override IOperation Insert(Entity entity) {
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

        public override Fields GetEntitySchema(Process process, string name, string schema = "", bool isMaster = false) {
            return new FieldInspector().Inspect(FileInformationFactory.Create(File), process.FileInspectionRequest);
        }

        public override IOperation Delete(Entity entity) {
            throw new NotImplementedException();
        }

        public override IOperation Extract(Entity entity, bool firstRun) {
            if (Is.Excel()) {
                return new FileExcelExtract(entity, this, entity.Top);
            }
            if (Is.Delimited()) {
                return new FileDelimitedExtract(this, entity, entity.Top);
            }
            return new FileFixedExtract(this, entity, entity.Top);
        }

        public FileConnection(ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            Type = ProviderType.File;
        }
    }
}