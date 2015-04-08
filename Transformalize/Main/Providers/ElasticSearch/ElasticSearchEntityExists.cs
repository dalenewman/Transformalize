namespace Transformalize.Main.Providers.ElasticSearch {
    public class ElasticSearchEntityExists : IEntityExists {
        public bool Exists(AbstractConnection connection, Entity entity) {
            var client = new ElasticSearchClientFactory().Create(connection, entity);
            return client.Client.IndicesExists(client.Index).HttpStatusCode == 200;
        }
    }
}