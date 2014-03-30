namespace Transformalize.Main.Providers.ElasticSearch
{
    public class ElasticSearchEntityExists : IEntityExists {
        public bool Exists(AbstractConnection connection, Entity entity) {
            return new ElasticSearchEntityRecordsExist().RecordsExist(connection, entity);
        }
    }
}