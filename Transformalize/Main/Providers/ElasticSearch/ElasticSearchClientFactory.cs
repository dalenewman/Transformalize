using Transformalize.Libs.Elasticsearch.Net;
using Transformalize.Libs.Elasticsearch.Net.Connection.Configuration;
using Transformalize.Libs.Elasticsearch.Net.ConnectionPool;
using Transformalize.Libs.Nest;
using Transformalize.Libs.Nest.Domain.Connection;

namespace Transformalize.Main.Providers.ElasticSearch {

    public static class ElasticSearchClientFactory {

        public static ElasticSearchNetClient Create(AbstractConnection connection, Entity entity) {
            TflLogger.Debug(entity.ProcessName, entity.Name, "Preparing Elasticsearch.NET client for {0}", connection.Uri());
            var pool = new SingleNodeConnectionPool(connection.Uri());
            var settings = new ConnectionConfiguration(pool);

            return new ElasticSearchNetClient(
                new ElasticsearchClient(settings),
                entity == null ? string.Empty : entity.ProcessName.ToLower(),
                entity == null ? string.Empty : entity.Alias.ToLower()
            );
        }

        public static NestClient CreateNest(AbstractConnection connection, Entity entity) {
            TflLogger.Debug(entity.ProcessName, entity.Name, "Preparing NEST client for {0}", connection.Uri());
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
