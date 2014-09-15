using Transformalize.Libs.Elasticsearch.Net;
using Transformalize.Libs.Elasticsearch.Net.Connection.Configuration;
using Transformalize.Libs.Elasticsearch.Net.ConnectionPool;
using Transformalize.Libs.NLog;

namespace Transformalize.Main.Providers.ElasticSearch {

    public static class ElasticSearchClientFactory
    {
        private static readonly Logger Log = LogManager.GetLogger("tfl");

        public static ElasticSearchClient Create(AbstractConnection connection, Entity entity) {
            Log.Debug("Preparing Elasticsearch client for {0}", connection.Uri());
            var pool = new SingleNodeConnectionPool(connection.Uri());
            var settings = new ConnectionConfiguration(pool);

            return new ElasticSearchClient(
                new ElasticsearchClient(settings),
                entity == null ? string.Empty : entity.ProcessName.ToLower(),
                entity == null ? string.Empty : entity.Alias.ToLower()
            );
        }

    }
}
