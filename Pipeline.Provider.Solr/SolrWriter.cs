using SolrNet;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Provider.Solr {

    public class SolrWriter : IWrite {

        private readonly OutputContext _context;
        private readonly ISolrOperations<Dictionary<string,object>> _solr;
        private readonly Field[] _fields;

        public SolrWriter(OutputContext context, ISolrOperations<Dictionary<string,object>> solr) {
            _context = context;
            _solr = solr;
            _fields = context.OutputFields.Where(f => f.Type != "byte[]").ToArray();
        }

        public void Write(IEnumerable<IRow> rows) {
            var fullCount = 0;
            var batchCount = (uint)0;

            foreach (var part in rows.Partition(_context.Entity.InsertSize)) {
                var docs = new List<Dictionary<string, object>>();
                foreach (var row in part) {
                    batchCount++;
                    fullCount++;
                    docs.Add(_fields.ToDictionary(field => field.Alias.ToLower(), field=>row[field]));
                }
                var response = _solr.AddRange(docs);

                _context.Increment(@by: batchCount);
                if (response.Status == 0) {
                    _context.Debug(() => $"{batchCount} to output");
                } else {
                    _context.Error("ah!");
                }
                batchCount = 0;
            }

            _solr.Commit();

            _context.Info($"{fullCount} to output");
        }
    }
}
