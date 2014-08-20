using System;
using Transformalize.Configuration;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Main.Providers.Lucene {
    public class LuceneConnection : AbstractConnection {
        public LuceneConnection(ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies) : base(element, dependencies) { }
        public override int NextBatchId(string processName) {
            throw new NotImplementedException();
        }

        public override void WriteEndVersion(AbstractConnection input, Entity entity, bool force = false) {
            throw new NotImplementedException();
        }

        public override IOperation EntityOutputKeysExtract(Entity entity) {
            throw new NotImplementedException();
        }

        public override IOperation EntityOutputKeysExtractAll(Entity entity) {
            throw new NotImplementedException();
        }

        public override IOperation EntityBulkLoad(Entity entity) {
            throw new NotImplementedException();
        }

        public override IOperation EntityBatchUpdate(Entity entity) {
            throw new NotImplementedException();
        }

        public override void LoadBeginVersion(Entity entity) {
            throw new NotImplementedException();
        }

        public override void LoadEndVersion(Entity entity) {
            throw new NotImplementedException();
        }

        public override Fields GetEntitySchema(Process process, string name, string schema = "", bool isMaster = false) {
            throw new NotImplementedException();
        }
    }
}