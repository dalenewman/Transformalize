using Transformalize.Configuration;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Operations;
using Transformalize.Operations.Transform;

namespace Transformalize.Main.Providers.Internal {

    public class InternalConnection : AbstractConnection {

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
            return new Fields();
        }

        public override IOperation Delete(Entity entity) {
            throw new System.NotImplementedException();
        }

        public override IOperation Extract(Process process, Entity entity, bool firstRun) {
            var p = new PartialProcessOperation(process);
            if (entity.InputOperation == null) {
                if (process.DataSets.ContainsKey(entity.Name)) {
                    // TODO: Make EntityFieldTypeConverterOperation
                    var converter = Common.GetObjectConversionMap();
                    var rows = process.DataSets[entity.Name];
                    foreach (var field in entity.Fields.WithInput()) {
                        if (field.SimpleType == "string") 
                            continue;
                        foreach (var row in rows) {
                            row[field.Name] = converter[field.SimpleType](row[field.Name]);
                        }
                    }
                    entity.InputOperation = new RowsOperation(process.DataSets[entity.Name]);
                } else {
                    entity.InputOperation = new EmptyOperation();
                }
            }
            p.Register(entity.InputOperation);
            p.Register(new AliasOperation(entity));
            return p;
        }

        public override string KeyAllQuery(Entity entity) {
            return string.Empty;
        }

        public InternalConnection(TflConnection element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            Type = ProviderType.Internal;
        }
    }
}