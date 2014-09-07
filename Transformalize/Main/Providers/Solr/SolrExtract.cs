using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Transformalize.Libs.Ninject.Syntax;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Libs.SolrNet;
using Transformalize.Libs.SolrNet.Commands.Parameters;

namespace Transformalize.Main.Providers.Solr {
    public class SolrExtract : AbstractOperation {
        private readonly SolrConnection _solrConnection;
        private readonly Fields _fields;
        private readonly string _alias;

        public SolrExtract(SolrConnection solrConnection, Entity entity) {
            _solrConnection = solrConnection;
            _fields = entity.Fields.WithInput();
            _alias = entity.Alias;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            var coreUrl = _solrConnection.GetCoreUrl(_alias);
            _solrConnection.RegisterCore(_alias);
            var solr = _solrConnection.Kernal.Get<ISolrReadOnlyOperations<Dictionary<string, object>>>(coreUrl);

            var count = solr.Query(SolrQuery.All, new QueryOptions { Start = 0, Rows = 0 }).NumFound;
            var solrResults = solr.Query(
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