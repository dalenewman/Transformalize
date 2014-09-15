using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Main.Providers.ElasticSearch {

    public class ElasticSearchEntityExtract : AbstractOperation {
        private readonly NestClient _client;
        private readonly string[] _fields;
        private int _count;

        public ElasticSearchEntityExtract(AbstractConnection connection, Entity entity, IEnumerable<string> fields) {
            _fields = fields.Select(a=>a.ToLower()).ToArray(); //for now
            _client = ElasticSearchClientFactory.CreateNest(connection, entity);
            Name = string.Format("ElasticsearchEntityExtract ({0}:{1})", _client.Index, _client.Type);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            OnRowProcessed += ElasticSearchEntityExtract_OnRowProcessed;

            var scanResults = _client.Client.Search<dynamic>(s => s
                .Index(_client.Index)
                .Type(_client.Type)
                .From(0)
                .Size(100)
                .MatchAll()
                .Fields(_fields)
                .SearchType(Libs.Elasticsearch.Net.Domain.SearchType.Scan)
                .Scroll("4s")
                );

            var results = _client.Client.Scroll<dynamic>(s => s
                .Scroll("2s")
                .ScrollId(scanResults.ScrollId)
                );
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
                    .Scroll("2s")
                    .ScrollId(localResults.ScrollId));
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