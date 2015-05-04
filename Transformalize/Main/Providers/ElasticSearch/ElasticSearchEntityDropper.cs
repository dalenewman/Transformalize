using Transformalize.Logging;

namespace Transformalize.Main.Providers.ElasticSearch {

    public class ElasticSearchEntityDropper : IEntityDropper {

        public IEntityExists EntityExists { get; set; }

        public ElasticSearchEntityDropper() {
            EntityExists = new ElasticSearchEntityExists();
        }

        public void Drop(AbstractConnection connection, Entity entity) {
            if (!EntityExists.Exists(connection, entity))
                return;

            var client = new ElasticSearchClientFactory().Create(connection, entity);
            var response = client.Client.IndicesDelete(client.Index);
            if (response.Success)
                return;

            connection.Logger.EntityWarn(entity.Name, response.ServerError.Error);
            connection.Logger.EntityWarn(entity.Name, "Trouble deleting {0} {1}.", client.Index, client.Type);
        }
    }
}