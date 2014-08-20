using Transformalize.Libs.NLog;

namespace Transformalize.Main.Providers.ElasticSearch {

    public class ElasticSearchEntityDropper : IEntityDropper {
        private readonly Logger _log = LogManager.GetLogger("tfl");
        public IEntityExists EntityExists { get; set; }

        public ElasticSearchEntityDropper() {
            EntityExists = new ElasticSearchEntityExists();
        }

        public void Drop(AbstractConnection connection, Entity entity) {
            if (!EntityExists.Exists(connection, entity))
                return;

            var client = ElasticSearchClientFactory.Create(connection, entity);
            var response = client.Client.IndicesDelete(client.Index);
            if (response.Success)
                return;

            _log.Warn(response.Error.ExceptionMessage);
            _log.Warn("Trouble deleting {0} {1}.", client.Index, client.Type);
        }
    }
}