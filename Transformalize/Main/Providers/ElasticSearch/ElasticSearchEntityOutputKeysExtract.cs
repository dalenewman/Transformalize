using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Main.Providers.ElasticSearch {
    public class ElasticSearchEntityOutputKeysExtract : AbstractOperation {
        private readonly List<AliasType> _aliasTypes = new List<AliasType>();
        private readonly string[] _sourceInclude;
        private readonly Dictionary<string, Func<object, object>> _conversionMap = Common.GetObjectConversionMap();
        private readonly ElasticSearchNetClient _client;

        public ElasticSearchEntityOutputKeysExtract(AbstractConnection connection, Entity entity) {
            _aliasTypes = entity.PrimaryKey.AliasTypes().ToList();
            if (entity.Version != null) {
                _aliasTypes.Add(new AliasType() { Alias = entity.Version.Alias, AliasLower = entity.Version.Alias.ToLower(), SimpleType = entity.Version.SimpleType });
            }
            _sourceInclude = _aliasTypes.Select(at => at.AliasLower).ToArray();
            _client = new ElasticSearchClientFactory().Create(connection, entity);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            var count = _client.Client.SearchGet(_client.Index, _client.Type, s => s
                .AddQueryString("q", "*:*")
                .AddQueryString("search_type", "count")
            );

            var result = _client.Client.SearchGet(_client.Index, _client.Type, s => s
                .AddQueryString("q", "*:*")
                .AddQueryString("_source_include", string.Join(",", _sourceInclude))
                .AddQueryString("size", count.Response["hits"].total)
            );

            var hits = result.Response["hits"].hits;
            for (var i = 0; i < result.Response["hits"].total; i++) {
                var row = new Row();
                foreach (var field in _aliasTypes) {
                    var value = hits[i]["_source"][field.AliasLower];
                    row[field.Alias] = _conversionMap[field.SimpleType](value);
                }
                yield return row;
            }
        }

    }
}
