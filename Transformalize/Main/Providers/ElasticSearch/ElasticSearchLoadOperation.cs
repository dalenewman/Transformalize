using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Transformalize.Extensions;
using Transformalize.Libs.fastJSON;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Main.Providers.ElasticSearch {

    public sealed class ElasticSearchLoadOperation : AbstractOperation {
        private readonly Entity _entity;

        private readonly ElasticSearchNetClient _client;
        private readonly string _prefix;
        private readonly bool _singleKey;
        private readonly string[] _keys;
        private readonly string _key;
        private int _count;
        private readonly int _batchSize;
        private readonly List<string> _guids = new List<string>();
        private readonly List<string> _dates = new List<string>();
        private readonly Dictionary<string, string> _elasticMap; 

        public ElasticSearchLoadOperation(Entity entity, AbstractConnection connection) {
            _entity = entity;

            _guids.AddRange(new Fields(entity.Fields, entity.CalculatedFields).WithOutput().WithGuid().Aliases());
            _dates.AddRange(new Fields(entity.Fields, entity.CalculatedFields).WithOutput().WithDate().Aliases());

            _client = new ElasticSearchClientFactory().Create(connection, entity);
            _prefix = "{\"index\": {\"_index\": \"" + _client.Index + "\", \"_type\": \"" + _client.Type + "\", \"_id\": \"";

            _singleKey = entity.PrimaryKey.Count == 1;
            _elasticMap = new ElasticSearchEntityCreator().GetFieldMap(entity);

            _keys = entity.PrimaryKey.Aliases().ToArray();
            _key = entity.FirstKey();
            _batchSize = connection.BatchSize;

        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

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
                    body.Append(_prefix);
                    body.Append(key);
                    body.AppendLine("\"}}");
                    body.AppendLine(JSON.Instance.ToJSON(_elasticMap.ToDictionary(item => item.Key.ToLower(), item => row[item.Value])));
                    Interlocked.Increment(ref _count);
                }
                _client.Client.Bulk(body.ToString(), nv => nv
                    .AddQueryString("refresh", @"true")
                );
                if (_count % LogRows == 0) {
                    TflLogger.Info(_entity.ProcessName, _entity.Name, "Processed {0} rows in {1}", _count, Name);
                }
            }
            yield break;

        }
    }
}