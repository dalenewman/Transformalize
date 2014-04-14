using System.Collections.Generic;
using Transformalize.Libs.fastJSON;
using Transformalize.Libs.NLog;

namespace Transformalize.Main.Providers.ElasticSearch {

    public class ElasticSearchEntityCreator : IEntityCreator {
        private readonly Logger _log = LogManager.GetLogger(string.Empty);
        private readonly Dictionary<string, string> _types = new Dictionary<string, string>() {
            {"int64", "long"},
            {"int16","integer"},
            {"int","integer"},
            {"int32","integer"},
            {"datetime","date"},
            {"bool","boolean"},
            {"decimal","float"},
            {"guid","string"}
        };
        public IEntityExists EntityExists { get; set; }

        public ElasticSearchEntityCreator() {
            EntityExists = new ElasticSearchEntityExists();
        }

        public void Create(AbstractConnection connection, Process process, Entity entity) {

            var client = ElasticSearchClientFactory.Create(connection, entity);

            client.Client.IndicesCreate(client.Index, "{ \"settings\":{}}");

            var fields = new Dictionary<string, object>();
            foreach (var field in entity.OutputFields()) {
                var alias = field.Alias.ToLower();
                var type = _types.ContainsKey(field.SimpleType) ? _types[field.SimpleType] : field.SimpleType;
                fields[alias] = new Dictionary<string, object>() { { "type", type } };
            }
            var properties = new Dictionary<string, object>() { { "properties", fields } };
            var body = new Dictionary<string, object>() { { client.Type, properties } };
            var json = JSON.Instance.ToJSON(body);

            var response = client.Client.IndicesPutMapping(client.Index, client.Type, json);

            if (response.Success)
                return;

            _log.Error(response.Error.ExceptionMessage);
            throw new TransformalizeException("Error writing ElasticSearch mapping.");
        }
    }
}