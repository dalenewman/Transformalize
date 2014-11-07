using System.Collections.Generic;
using Transformalize.Libs.fastJSON;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.ElasticSearch {
    public class ElasticSearchEntityCreator : IEntityCreator {

        private readonly Dictionary<string, string> _types = new Dictionary<string, string>() {
            {"int64", "long"},
            {"int16","integer"},
            {"int","integer"},
            {"int32","integer"},
            {"datetime","date"},
            {"time","date"},
            {"bool","boolean"},
            {"decimal","float"},
            {"single","double"},
            {"guid","string"},
            {"byte[]","binary"}
        };

        private readonly List<string> _analyzers = new List<string> {
            "standard",
            "simple",
            "whitespace",
            "keyword",
            "pattern",
            "snowball",
            "arabic",
            "armenian",
            "basque",
            "brazilian",
            "bulgarian",
            "catalan",
            "chinese",
            "cjk",
            "czech",
            "danish",
            "dutch",
            "english",
            "finnish",
            "french",
            "galician",
            "german",
            "greek",
            "hindi",
            "hungarian",
            "indonesian",
            "italian",
            "norwegian",
            "persian",
            "portuguese",
            "romanian",
            "russian",
            "spanish",
            "swedish",
            "turkish",
            "thai",
            string.Empty
        };
        public IEntityExists EntityExists { get; set; }

        public ElasticSearchEntityCreator() {
            EntityExists = new ElasticSearchEntityExists();
        }

        public void Create(AbstractConnection connection, Process process, Entity entity) {

            var client = new ElasticSearchClientFactory().Create(connection, entity);

            client.Client.IndicesCreate(client.Index, "{ \"settings\":{}}");

            var fields = GetFields(entity);
            var properties = new Dictionary<string, object>() { { "properties", fields } };
            var body = new Dictionary<string, object>() { { client.Type, properties } };
            var json = JSON.Instance.ToJSON(body);

            var response = client.Client.IndicesPutMapping(client.Index, client.Type, json);

            if (response.Success)
                return;

            TflLogger.Error(process.Name, entity.Name, response.ServerError.Error);
            throw new TransformalizeException("Error writing ElasticSearch mapping.");
        }

        public Dictionary<string, object> GetFields(Entity entity) {
            var fields = new Dictionary<string, object>();
            foreach (var field in entity.OutputFields()) {
                var alias = field.Alias.ToLower();
                var type = _types.ContainsKey(field.SimpleType) ? _types[field.SimpleType] : field.SimpleType;
                if (type.Equals("string")) {
                    foreach (var searchType in field.SearchTypes) {
                        var analyzer = searchType.Analyzer.ToLower();
                        if (_analyzers.Contains(analyzer)) {
                            if (fields.ContainsKey(alias)) {
                                fields[alias + searchType.Name.ToLower()] = new Dictionary<string, object>() { { "type", type }, { "analyzer", analyzer } };
                            } else {
                                if (analyzer.Equals(string.Empty)) {
                                    fields[alias] = new Dictionary<string, object>() { { "type", type } };
                                } else {
                                    fields[alias] = new Dictionary<string, object>() { { "type", type }, { "analyzer", analyzer } };
                                }
                            }
                        } else {
                            TflLogger.Warn(entity.ProcessName, entity.Name, "Analyzer '{0}' specified in search type '{1}' is not supported.  Please use a built-in analyzer for Elasticsearch.", analyzer, searchType.Name);
                            if (!fields.ContainsKey(alias)) {
                                fields[alias] = new Dictionary<string, object>() { { "type", type } };
                            }
                        }
                    }
                } else {
                    fields[alias] = new Dictionary<string, object>() { { "type", type } };
                }
            }
            if (!fields.ContainsKey("tflbatchid")) {
                fields.Add("tflbatchid", new Dictionary<string, object> { { "type", "long" } });
            }
            if (!fields.ContainsKey("tfldeleted")) {
                fields.Add("tfldeleted", new Dictionary<string, object> { { "type", "boolean" } });
            }
            return fields;
        }

        public Dictionary<string, string> GetFieldMap(Entity entity) {
            var map = new Dictionary<string, string>();
            foreach (var field in entity.OutputFields()) {
                var aliasLower = field.Alias.ToLower();
                if (field.SimpleType.Equals("string")) {
                    foreach (var searchType in field.SearchTypes) {
                        if (map.ContainsKey(aliasLower)) {
                            map[aliasLower + searchType.Name.ToLower()] = field.Alias;
                        } else {
                            map[aliasLower] = field.Alias;
                        }
                    }
                } else {
                    map[aliasLower] = field.Alias;
                }
            }
            if (!map.ContainsKey("tflbatchid")) {
                map.Add("tflbatchid", "tflbatchid");
            }
            if (!map.ContainsKey("tfldeleted")) {
                map.Add("tfldeleted", "tfldeleted");
            }
            return map;
        }

    }
}