using Transformalize.Libs.Elasticsearch.Net;
using Transformalize.Libs.Elasticsearch.Net.Connection.Configuration;
using Transformalize.Libs.Elasticsearch.Net.ConnectionPool;
using Transformalize.Libs.Nest;
using Transformalize.Libs.Nest.Domain.Connection;

namespace Transformalize.Main.Providers.ElasticSearch {

    public class ElasticSearchClientFactory {

        public ElasticSearchNetClient Create(AbstractConnection connection, Entity entity) {
            var entityExists = entity != null;
            var processName = entityExists ? entity.ProcessName : string.Empty;
            var entityName = entityExists ? entity.Name : string.Empty;
            var alias = entityExists ? entity.Alias.ToLower() : string.Empty;

            TflLogger.Debug(processName, entityName, "Preparing Elasticsearch.NET client for {0}", connection.Uri());
            var settings = new ConnectionConfiguration(
                new SingleNodeConnectionPool(connection.Uri())
            );

            return new ElasticSearchNetClient(
                new ElasticsearchClient(settings),
                processName.ToLower(),
                alias
            );
        }

        public NestClient CreateNest(AbstractConnection connection, Entity entity) {

            var entityExists = entity != null;
            var processName = entityExists ? entity.ProcessName : string.Empty;
            var entityName = entityExists ? entity.Name : string.Empty;
            var alias = entityExists ? entity.Alias.ToLower() : string.Empty;

            TflLogger.Debug(processName, entityName, "Preparing NEST client for {0}", connection.Uri());
            var pool = new SingleNodeConnectionPool(connection.Uri());
            var settings = new ConnectionSettings(pool).SetTimeout(System.Threading.Timeout.Infinite);

            return new NestClient(
                new ElasticClient(settings),
                processName.ToLower(),
                alias
            );
        }

    }
}
