using System;
using Transformalize.Configuration;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Processes;

namespace Transformalize.Main.Providers.ElasticSearch {

    public class ElasticSearchConnection : AbstractConnection {
        private readonly Process _process;

        public override string UserProperty { get { return string.Empty; } }
        public override string PasswordProperty { get { return string.Empty; } }
        public override string PortProperty { get { return string.Empty; } }
        public override string DatabaseProperty { get { return string.Empty; } }
        public override string ServerProperty { get { return string.Empty; } }
        public override string TrustedProperty { get { return string.Empty; } }
        public override string PersistSecurityInfoProperty { get { return string.Empty; } }

        public ElasticSearchConnection(Process process, ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            _process = process;
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
                var value = (int)hits[i]["_source"]["tflbatchid"];
                if (value > max)
                    max = value;
            }
            return max + 1;
        }

        public override void WriteEndVersion(AbstractConnection input, Entity entity) {
            if (entity.Inserts + entity.Updates > 0 || _process.IsFirstRun) {
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
        }

        public override IOperation EntityOutputKeysExtract(Entity entity) {
            return new ElasticSearchEntityOutputKeysExtract(this, entity);
        }

        public override IOperation EntityOutputKeysExtractAll(Entity entity) {
            return new ElasticSearchEntityOutputKeysExtract(this, entity);
        }

        public override IOperation EntityBulkLoad(Entity entity) {
            return new ElasticSearchLoadOperation(entity, this);
        }

        public override IOperation EntityBatchUpdate(Entity entity) {
            return new ElasticSearchLoadOperation(entity, this);
        }

        public override void LoadBeginVersion(Entity entity) {
            //get the version and type for the maximum tflbatchid for a given process and entity
            //set entity.HasRange
            //set entity.Begin
            var client = ElasticSearchClientFactory.Create(this, TflBatchEntity(entity.ProcessName));
            var body = new {
                query = new {
                    query_string = new {
                        query = entity.Alias,
                        fields = new[] { "entity" }
                    }
                },
                aggs = new {
                    tflbatchid = new {
                        max = new {
                            field = "tflbatchid"
                        }
                    }
                },
                size = 0
            };
            var result = client.Client.Search(client.Index, client.Type, body);
            var tflbatchid = result.Response["aggregations"]["tflbatchid"]["value"].Value;

            if (tflbatchid != null) {
                var tflBatchId = Convert.ToInt32(tflbatchid);
                if (tflBatchId > 0) {
                    result = client.Client.SearchGet(client.Index, client.Type, s => s
                        .Add("q", "tflbatchid=" + tflbatchid)
                        .Add("_source_include", "version,version_type")
                        .Add("size", 1)
                    );
                    var hits = result.Response["hits"].hits;
                    var versionType = (string)hits[0]["_source"]["version_type"].Value;
                    entity.Begin = Common.GetObjectConversionMap()[versionType](hits[0]["_source"]["version"].Value);
                    entity.HasRange = true;
                }
            }
        }

        public override void LoadEndVersion(Entity entity) {
            throw new NotImplementedException();
        }
    }
}