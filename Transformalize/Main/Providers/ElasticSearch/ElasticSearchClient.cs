using Transformalize.Libs.Elasticsearch.Net;

namespace Transformalize.Main.Providers.ElasticSearch
{
    public class ElasticSearchClient {
        public ElasticsearchClient Client { get; set; }
        public string Index { get; set; }
        public string Type { get; set; }

        public ElasticSearchClient(ElasticsearchClient client, string index, string type) {
            Client = client;
            Index = index;
            Type = type;
        }
    }
}