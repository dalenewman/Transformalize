using Transformalize.Libs.Nest;

namespace Transformalize.Main.Providers.ElasticSearch {
    public class NestClient {
        public ElasticClient Client { get; set; }
        public string Index { get; set; }
        public string Type { get; set; }

        public NestClient(ElasticClient client, string index, string type) {
            Client = client;
            Index = index;
            Type = type;
        }
    }
}