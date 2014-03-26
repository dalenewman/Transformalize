using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Elasticsearch.Net;
using Elasticsearch.Net.Connection;
using Elasticsearch.Net.ConnectionPool;
using Transformalize.Extensions;
using Transformalize.Libs.fastJSON;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Processes {
    public sealed class ElasticSearchLoadOperation : AbstractOperation {

        private readonly ElasticsearchClient _client;
        private readonly string _prefix;
        private readonly bool _singleKey;
        private readonly string[] _columns;
        private readonly string[] _keys;
        private readonly string _key;
        private int _count;
        private readonly int _batchSize;

        public ElasticSearchLoadOperation(Entity entity, AbstractConnection connection) {

            var builder = new UriBuilder(connection.Server.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? connection.Server : "http://" + connection.Server);
            if (connection.Port > 0) {
                builder.Port = connection.Port;
            }
            var pool = new SingleNodeConnectionPool(builder.Uri);
            var settings = new ConnectionConfiguration(pool);

            _client = new ElasticsearchClient(settings);
            _prefix = "{\"index\": {\"_index\": \"" + entity.ProcessName.ToLower() + "\", \"_type\": \"" + entity.Alias.ToLower() + "\", \"_id\": \"";

            _singleKey = entity.PrimaryKey.Count == 1;
            _columns = entity.OutputFields().Where(f => entity.PrimaryKey.Any(kv => !kv.Key.Equals(f.Alias))).Select(f => f.Alias).ToArray();

            _keys = entity.PrimaryKey.Select(kv => kv.Key).ToArray();
            _key = entity.FirstKey();
            _batchSize = connection.BatchSize;

            OnRowProcessed += ElasticSearchLoadOperation_OnRowProcessed;

        }

        void ElasticSearchLoadOperation_OnRowProcessed(IOperation arg1, Row arg2) {
            Interlocked.Increment(ref _count);
            if (_count % 100 == 0) {
                Debug("Processed {0} records.", _count);
            } else if (_count % 1000 == 0) {
                Info("Processed {0} records.", _count);
            }
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            foreach (var batch in rows.Partition(_batchSize)) {
                var body = new StringBuilder();
                foreach (var row in batch) {
                    var r = row;
                    var key = _singleKey ? row[_key].ToString() : string.Concat(_keys.Select(k => r[k].ToString()));
                    body.Append(_prefix);
                    body.Append(key);
                    body.AppendLine("\"}}");
                    body.AppendLine(JSON.Instance.ToJSON(_columns.ToDictionary(alias => alias.ToLower(), alias => row[alias])));
                }
                _client.Bulk(body.ToString(), nv => nv
                    .Add("refresh", @"true")
                );
            }
            yield break;

        }
    }
}