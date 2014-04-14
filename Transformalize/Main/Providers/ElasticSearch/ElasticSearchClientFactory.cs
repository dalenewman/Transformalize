using System;
using Transformalize.Libs.Elasticsearch.Net;
using Transformalize.Libs.Elasticsearch.Net.Connection;
using Transformalize.Libs.Elasticsearch.Net.ConnectionPool;
using Transformalize.Libs.NLog;

namespace Transformalize.Main.Providers.ElasticSearch {

    public static class ElasticSearchClientFactory
    {
        private static readonly Logger Log = LogManager.GetLogger(string.Empty);

        public static ElasticSearchClient Create(AbstractConnection connection, Entity entity) {
            var builder = new UriBuilder(connection.Server.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? connection.Server : "http://" + connection.Server);
            if (connection.Port > 0) {
                builder.Port = connection.Port;
            }
            Log.Debug("Creating connection to {0}.", builder.Uri);
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
