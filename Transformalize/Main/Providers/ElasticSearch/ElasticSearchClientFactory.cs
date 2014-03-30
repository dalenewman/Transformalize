using System;
using Transformalize.Libs.Elasticsearch.Net;
using Transformalize.Libs.Elasticsearch.Net.Connection;
using Transformalize.Libs.Elasticsearch.Net.ConnectionPool;

namespace Transformalize.Main.Providers.ElasticSearch {
    static class ElasticSearchClientFactory {

        public static ElasticSearchClient Create(AbstractConnection connection, Entity entity) {
            var builder = new UriBuilder(connection.Server.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? connection.Server : "http://" + connection.Server);
            if (connection.Port > 0) {
                builder.Port = connection.Port;
            }
            var pool = new SingleNodeConnectionPool(builder.Uri);
            var settings = new ConnectionConfiguration(pool);

            return new ElasticSearchClient(
                new ElasticsearchClient(settings),
                entity == null ? string.Empty : entity.ProcessName.ToLower(),
                entity == null ? string.Empty : entity.Alias.ToLower()
            );
        }

    }
}
