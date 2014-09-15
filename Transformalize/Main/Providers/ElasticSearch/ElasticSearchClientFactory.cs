using Transformalize.Libs.Elasticsearch.Net;
using Transformalize.Libs.Elasticsearch.Net.Connection.Configuration;
using Transformalize.Libs.Elasticsearch.Net.ConnectionPool;
using Transformalize.Libs.Nest;
using Transformalize.Libs.Nest.Domain.Connection;
using Transformalize.Libs.NLog;

namespace Transformalize.Main.Providers.ElasticSearch {

    public static class ElasticSearchClientFactory {
        private static readonly Logger Log = LogManager.GetLogger("tfl");

        public static ElasticSearchNetClient Create(AbstractConnection connection, Entity entity) {
            Log.Debug("Preparing Elasticsearch.NET client for {0}", connection.Uri());
            var pool = new SingleNodeConnectionPool(connection.Uri());
            var settings = new ConnectionConfiguration(pool);

            return new ElasticSearchNetClient(
                new ElasticsearchClient(settings),
                entity == null ? string.Empty : entity.ProcessName.ToLower(),
                entity == null ? string.Empty : entity.Alias.ToLower()
            );
        }

        public static NestClient CreateNest(AbstractConnection connection, Entity entity) {
            Log.Debug("Preparing NEST client for {0}", connection.Uri());
            var pool = new SingleNodeConnectionPool(connection.Uri());
            var settings = new ConnectionSettings(pool);

            return new NestClient(
                new ElasticClient(settings),
                entity == null ? string.Empty : entity.ProcessName.ToLower(),
                entity == null ? string.Empty : entity.Alias.ToLower()
                );
        }

    }
}
