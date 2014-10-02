namespace Transformalize.Main.Providers.ElasticSearch {
    public class ElasticSearchEntityExists : IEntityExists {
        public bool Exists(AbstractConnection connection, Entity entity) {
            var client = new ElasticSearchClientFactory().Create(connection, entity);
            var response = client.Client.IndicesExists(client.Index);
            return response.Success;
        }
    }
}