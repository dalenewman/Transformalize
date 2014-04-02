namespace Transformalize.Main.Providers.ElasticSearch {
    public class ElasticSearchEntityRecordsExist : IEntityRecordsExist {

        public bool RecordsExist(AbstractConnection connection, Entity entity) {

            var checker = new ElasticSearchConnectionChecker();
            if (checker.Check(connection)) {
                var client = ElasticSearchClientFactory.Create(connection, entity);
                const string body = @"{ _source: false, from: 0, size:1, query: { match_all: {} } }";
                var response = client.Client.Search(client.Index, client.Type, body);
                if (!response.Success)
                    return false;

                var result = response.Response;
                return result["hits"].total > 0;
            }
            return false;

        }
    }
}