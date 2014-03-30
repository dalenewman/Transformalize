using Transformalize.Libs.NLog;

namespace Transformalize.Main.Providers.ElasticSearch
{
    public class ElasticSearchEntityDropper : IEntityDropper {
        private readonly Logger _log = LogManager.GetLogger(string.Empty);
        public IEntityExists EntityExists { get; set; }

        public void Drop(AbstractConnection connection, Entity entity) {
            var client = ElasticSearchClientFactory.Create(connection, entity);
            const string body = @"{ query: { match_all: {} } }";
            var response = client.Client.DeleteByQuery(client.Index, client.Type, body);
            if (!response.Success) {
                _log.Warn("Trouble deleting {0} {1}.", client.Index, client.Type);
            }
        }
    }
}