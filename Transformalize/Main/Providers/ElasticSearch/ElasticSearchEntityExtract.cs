using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Transformalize.Extensions;
using Transformalize.Libs.Nest.Domain.Responses;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Main.Providers.ElasticSearch {

    public class ElasticSearchEntityExtract : AbstractOperation {
        private readonly Entity _entity;
        private readonly bool _correspondingKeys;
        private readonly NestClient _client;
        private readonly string[] _fields;
        private int _count;

        public ElasticSearchEntityExtract(AbstractConnection connection, Entity entity, IEnumerable<string> fields, bool correspondingKeys = false) {
            _entity = entity;
            _correspondingKeys = correspondingKeys;

            _fields = fields.Select(a => a.ToLower()).ToArray(); //for now
            _client = new ElasticSearchClientFactory().CreateNest(connection, entity);
            Name = string.Format("ElasticsearchEntityExtract ({0}:{1})", _client.Index, _client.Type);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            OnRowProcessed += ElasticSearchEntityExtract_OnRowProcessed;

            ISearchResponse<dynamic> scanResults;
            ISearchResponse<dynamic> results;
            var scrolling = true;
            var size = _entity.InputKeys.Length;

            if (_correspondingKeys) {

                if (size == 0) {
                    if (rows == null)
                        yield break;
                    foreach (var row in rows) {
                        yield return row;
                    }
                }

                TflLogger.Info(_entity.ProcessName, _entity.Name, "Extract {0} corresponding key{1}", _entity.InputKeys.Length, _entity.InputKeys.Length.Plural());
                var queries = new List<string>();
                foreach (var row in _entity.InputKeys) {
                    var subQuery = new List<string>();
                    foreach (var key in _entity.PrimaryKey) {
                        subQuery.Add(string.Format("{0}:{1}", key.AliasLower, row[key.AliasLower]));
                    }
                    queries.Add("( " + string.Join(" AND ", subQuery) + " )");
                }
                var query = string.Join(" OR ", queries);

                TflLogger.Debug(_entity.ProcessName, _entity.Name, query);

                if (size < 100) {
                    TflLogger.Info(_entity.ProcessName, _entity.Name, "Using query to fetch results.");
                    scrolling = false;
                    results = _client.Client.Search<dynamic>(s => s
                        .Index(_client.Index)
                        .Type(_client.Type)
                        .From(0)
                        .Size(size)
                        .QueryString(query)
                        .SearchType(Libs.Elasticsearch.Net.Domain.SearchType.Scan)
                        .Fields(_fields)
                    );
                } else {
                    TflLogger.Info(_entity.ProcessName, _entity.Name, "Using query scroll to fetch results.");
                    scanResults = _client.Client.Search<dynamic>(s => s
                        .Index(_client.Index)
                        .Type(_client.Type)
                        .From(0)
                        .Size(100)
                        .QueryString(query)
                        .Fields(_fields)
                        .SearchType(Libs.Elasticsearch.Net.Domain.SearchType.Scan)
                        .Scroll("1m")
                        );
                    results = _client.Client.Scroll<dynamic>(s => s
                        .Scroll("5s")
                        .ScrollId(scanResults.ScrollId)
                        );
                }
            } else {
                TflLogger.Info(_entity.ProcessName, _entity.Name, "Using match all scroll to fetch results.");
                scanResults = _client.Client.Search<dynamic>(s => s
                    .Index(_client.Index)
                    .Type(_client.Type)
                    .From(0)
                    .Size(100)
                    .MatchAll()
                    .Fields(_fields)
                    .SearchType(Libs.Elasticsearch.Net.Domain.SearchType.Scan)
                    .Scroll("1m")
                    );
                results = _client.Client.Scroll<dynamic>(s => s
                    .Scroll("5s")
                    .ScrollId(scanResults.ScrollId)
                    );
            }

            if (scrolling) {
                while (results.FieldSelections.Any()) {
                    var localResults = results;
                    foreach (var hit in localResults.Hits) {
                        var row = new Row();
                        foreach (var pair in hit.Fields.FieldValuesDictionary) {
                            var value = hit.Fields.FieldValues<object[]>(pair.Key);
                            row[pair.Key] = value[0];
                        }
                        yield return row;
                    }
                    results = _client.Client.Scroll<dynamic>(s => s
                        .Scroll("5s")
                        .ScrollId(localResults.ScrollId));
                }
            } else {
                foreach (var hit in results.Hits) {
                    var row = new Row();
                    foreach (var pair in hit.Fields.FieldValuesDictionary) {
                        var value = hit.Fields.FieldValues<object[]>(pair.Key);
                        row[pair.Key] = value[0];
                    }
                    yield return row;
                }
            }
        }

        void ElasticSearchEntityExtract_OnRowProcessed(IOperation arg1, Row arg2) {
            Interlocked.Increment(ref _count);
            if (_count % arg1.LogRows == 0) {
                Info("Processed {0} records.", _count);
            }
        }

    }
}