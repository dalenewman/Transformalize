using Transformalize.Configuration;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main.Parameters;
using Transformalize.Operations;

namespace Transformalize.Main.Providers.Mail {

    public class WebConnection : AbstractConnection {
        private readonly ConnectionConfigurationElement _element;

        public WebConnection(ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            _element = element;
            Type = ProviderType.Web;
        }

        public override int NextBatchId(string processName) {
            return 1;
        }

        public override void WriteEndVersion(Process process, AbstractConnection input, Entity entity, bool force = false) {
            throw new System.NotImplementedException();
        }

        public override IOperation ExtractCorrespondingKeysFromOutput(Entity entity) {
            throw new System.NotImplementedException();
        }

        public override IOperation ExtractAllKeysFromOutput(Entity entity) {
            throw new System.NotImplementedException();
        }

        public override IOperation ExtractAllKeysFromInput(Process process, Entity entity) {
            throw new System.NotImplementedException();
        }

        public override IOperation Insert(Process process, Entity entity) {
            throw new System.NotImplementedException();
        }

        public override IOperation Update(Entity entity) {
            throw new System.NotImplementedException();
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
            var outKey = entity.Fields.First().Alias;
            var url = new Parameter("url", _element.Url == string.Empty ? _element.NormalizeUrl(80) : _element.Url);
            var data = new Parameter("data", _element.Data == Common.DefaultValue ? string.Empty : _element.Data);

            var partial = new PartialProcessOperation(process);
            var rows = new Row[1];
            rows[0] = new Row();

            partial.Register(new RowsOperation(rows));
            partial.Register(new WebOperation(url, outKey, 0, _element.WebMethod, data, _element.ContentType));

            return partial;
        }

    }
}