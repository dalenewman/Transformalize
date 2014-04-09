using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Libs.Elasticsearch.Net.Domain;
using Transformalize.Libs.fastJSON;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Operations.Transform;
using Transformalize.Processes;

namespace Transformalize.Main.Providers.ElasticSearch {

    public class ElasticSearchConnection : AbstractConnection {

        public override string UserProperty { get { return string.Empty; } }
        public override string PasswordProperty { get { return string.Empty; } }
        public override string PortProperty { get { return string.Empty; } }
        public override string DatabaseProperty { get { return string.Empty; } }
        public override string ServerProperty { get { return string.Empty; } }
        public override string TrustedProperty { get { return string.Empty; } }
        public override string PersistSecurityInfoProperty { get { return string.Empty; } }

        public ElasticSearchConnection(Process process, ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            TypeAndAssemblyName = process.Providers[element.Provider.ToLower()];
            Type = ProviderType.ElasticSearch;
            IsDatabase = true;
        }

        public override int NextBatchId(string processName) {
            if (!TflBatchRecordsExist(processName)) {
                return 1;
            }

            var client = ElasticSearchClientFactory.Create(this, TflBatchEntity(processName));
            var result = client.Client.SearchGet(client.Index, client.Type, s => (s
                .Add("q", string.Format("process:{0}", processName))
                .Add("_source_include", "tflbatchid")
                .Add("size", 10000000)
            ));

            //this just can't be how you're suppose to do it...
            var max = 1;
            var hits = result.Response["hits"].hits;
            for (var i = 0; i < result.Response["hits"].total; i++) {
                var value = (int) hits[i]["_source"]["tflbatchid"];
                if (value > max)
                    max = value;
            }
            return max + 1;
        }

        public override void WriteEndVersion(AbstractConnection input, Entity entity) {
            var client = ElasticSearchClientFactory.Create(this, TflBatchEntity(entity.ProcessName));
            var versionType = entity.Version == null ? "string" : entity.Version.SimpleType;
            var body = new {
                id = entity.TflBatchId,
                tflbatchid = entity.TflBatchId,
                process = entity.ProcessName,
                connection = input.Name,
                entity = entity.Alias,
                updates = entity.Updates,
                inserts = entity.Inserts,
                deletes = entity.Deletes,
                version = new DefaultFactory().Convert(entity.End, versionType),
                version_type = versionType,
                tflupdate = DateTime.UtcNow
            };
            client.Client.Index(client.Index, client.Type, body);
        }

        public override IOperation EntityOutputKeysExtract(Entity entity) {
            return new ElasticSearchEntityOutputKeysExtract(this, entity);
        }

        public override IOperation EntityBulkLoad(Entity entity) {
            return new ElasticSearchLoadOperation(entity, this);
        }

        public override IOperation EntityBatchUpdate(Entity entity) {
            return new EmptyOperation();
        }
    }

    public class ElasticSearchEntityOutputKeysExtract : AbstractOperation {
        private readonly AbstractConnection _connection;
        private readonly Entity _entity;
        private readonly List<AliasType> _aliasTypes = new List<AliasType>();
        private readonly string[] _sourceInclude;
        private readonly Dictionary<string, Func<object, object>> _conversionMap = Common.GetObjectConversionMap();

        private struct AliasType {
            public string Alias;
            public string SimpleType;
        }

        public ElasticSearchEntityOutputKeysExtract(AbstractConnection connection, Entity entity) {
            _connection = connection;
            _entity = entity;
            _aliasTypes = _entity.PrimaryKey.Select(f => new AliasType() { Alias = f.Value.Alias, SimpleType = f.Value.SimpleType }).ToList();
            if (_entity.Version != null) {
                _aliasTypes.Add(new AliasType() { Alias = _entity.Version.Alias, SimpleType = _entity.Version.SimpleType });
            }
            _sourceInclude = _aliasTypes.Select(at => at.Alias).ToArray();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            var client = ElasticSearchClientFactory.Create(_connection, _entity);
            var result = client.Client.SearchGet(client.Index, client.Type, s => s
                .Add("q", "*:*")
                .Add("_source_include", string.Join(",", _sourceInclude))
                .Add("size", 10000000)
            );
            var hits = result.Response["hits"].hits;
            for (var i = 0; i < result.Response["hits"].total; i++) {
                var row = new Row();
                foreach (var field in _aliasTypes) {
                    var value = hits[i]["_source"][field.Alias];
                    row[field.Alias] = _conversionMap[field.SimpleType](value);
                    yield return row;
                }
            }
        }
    }
}