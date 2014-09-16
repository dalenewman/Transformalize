using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Libs.SolrNet;
using Transformalize.Libs.SolrNet.Commands.Parameters;

namespace Transformalize.Main.Providers.Solr {
    public class SolrExtract : AbstractOperation {
        private readonly Fields _fields;
        private readonly ISolrReadOnlyOperations<Dictionary<string, object>> _solr;

        public SolrExtract(Process process, SolrConnection solrConnection, Fields fields, string core) {
            _solr = solrConnection.GetReadonlyOperations(process, core);
            _fields = fields;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            var count = _solr.Query(SolrQuery.All, new QueryOptions { Start = 0, Rows = 0 }).NumFound;
            var solrResults = _solr.Query(
                SolrQuery.All,
                new QueryOptions {
                    Start = 0,
                    Rows = count,
                    Fields = new Collection<string>(_fields.Select(f => f.Name).ToList()),
                });

            foreach (var doc in solrResults) {
                var row = new Row();
                foreach (var pair in doc) {
                    row[pair.Key] = pair.Value;
                }
                yield return row;
            }
        }
    }
}