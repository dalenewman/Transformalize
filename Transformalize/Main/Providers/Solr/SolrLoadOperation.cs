using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Transformalize.Extensions;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Main.Providers.Solr {

    public sealed class SolrLoadOperation : AbstractOperation {

        private readonly bool _singleKey;
        private readonly string[] _keys;
        private readonly string _key;
        private int _count;
        private readonly int _batchSize;
        private readonly List<string> _guids = new List<string>();
        private readonly List<string> _dates = new List<string>();
        private readonly Dictionary<string, string> _solrMap; 

        public SolrLoadOperation(Entity entity, AbstractConnection connection) {

            _guids.AddRange(new FieldSqlWriter(entity.Fields, entity.CalculatedFields).Output().ToArray().Where(f => f.SimpleType.Equals("guid")).Select(f => f.Alias));
            _dates.AddRange(new FieldSqlWriter(entity.Fields, entity.CalculatedFields).Output().ToArray().Where(f => f.SimpleType.StartsWith("date")).Select(f => f.Alias));

            _singleKey = entity.PrimaryKey.Count == 1;
            _solrMap = new SolrEntityCreator().GetFieldMap(entity);

            _keys = entity.PrimaryKey.Select(kv => kv.Key).ToArray();
            _key = entity.FirstKey();
            _batchSize = connection.BatchSize;

        }

        void SolrLoadOperation_OnRowProcessed(IOperation arg1, Row arg2) {
            Interlocked.Increment(ref _count);
            if (_count % 1000 == 0) {
                Debug("Processed {0} records.", _count);
            } else if (_count % 10000 == 0) {
                Info("Processed {0} records.", _count);
            }
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            OnRowProcessed += SolrLoadOperation_OnRowProcessed;

            foreach (var batch in rows.Partition(_batchSize)) {
                var body = new StringBuilder();
                foreach (var row in batch) {
                    foreach (var guid in _guids) {
                        row[guid] = ((Guid)row[guid]).ToString();
                    }
                    foreach (var date in _dates) {
                        row[date] = ((DateTime)row[date]).ToString("yyyy-MM-ddTHH:mm:ss.fff");
                    }
                    var r = row;
                    var key = _singleKey ? row[_key].ToString() : string.Concat(_keys.Select(k => r[k].ToString()));

                }
            }
            yield break;

        }
    }
}