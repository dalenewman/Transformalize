using System;
using Transformalize.Configuration;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Operations.Transform;

namespace Transformalize.Main.Providers.ElasticSearch {
    public class ElasticSearchConnection : AbstractConnection {

        public ElasticSearchConnection(ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            Type = ProviderType.ElasticSearch;
            IsDatabase = true;
        }

        public override int NextBatchId(string processName) {
            if (!TflBatchRecordsExist(processName)) {
                return 1;
            }
            return GetMaxTflBatchId(processName) + 1;
        }

        public override void WriteEndVersion(AbstractConnection input, Entity entity, bool force = false) {
            if (entity.Updates + entity.Inserts > 0 || force) {
                var client = ElasticSearchClientFactory.Create(this, TflBatchEntity(entity.ProcessName));
                var versionType = entity.Version == null ? "string" : entity.Version.SimpleType;
                var end = versionType.Equals("byte[]") || versionType.Equals("rowversion") ? Common.BytesToHexString((byte[])entity.End) : new DefaultFactory().Convert(entity.End, versionType).ToString();
                var body = new {
                    id = entity.TflBatchId,
                    tflbatchid = entity.TflBatchId,
                    process = entity.ProcessName,
                    connection = input.Name,
                    entity = entity.Alias,
                    updates = entity.Updates,
                    inserts = entity.Inserts,
                    deletes = entity.Deletes,
                    version = end,
                    version_type = versionType,
                    tflupdate = DateTime.UtcNow
                };
                client.Client.Index(client.Index, client.Type, body);
            }
        }

        public override IOperation EntityOutputKeysExtract(Entity entity) {
            return new EmptyOperation();
            //need to learn search type scan / scroll functionality for bigger result sets
            //return new ElasticSearchEntityOutputKeysExtract(this, entity);
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
            var tflBatchId = GetMaxTflBatchId(entity);
            if (tflBatchId > 0) {
                var client = ElasticSearchClientFactory.Create(this, TflBatchEntity(entity.ProcessName));
                var result = client.Client.SearchGet(client.Index, client.Type, s => s
                    .Add("q", "tflbatchid:" + tflBatchId)
                    .Add("_source_include", "version,version_type")
                    .Add("size", 1)
                );
                var hits = result.Response["hits"].hits;
                var versionType = (string)hits[0]["_source"]["version_type"].Value;
                entity.Begin = Common.GetObjectConversionMap()[versionType](hits[0]["_source"]["version"].Value);
                entity.HasRange = true;
            }
        }

        public override void LoadEndVersion(Entity entity) {

            var client = ElasticSearchClientFactory.Create(this, entity);
            var body = new {
                aggs = new {
                    version = new {
                        max = new {
                            field = entity.Version.Alias.ToLower()
                        }
                    }
                },
                size = 0
            };
            var result = client.Client.Search(client.Index, client.Type, body);
            entity.End = Common.GetObjectConversionMap()[entity.Version.SimpleType](result.Response["aggregations"]["version"]["value"].Value);
            entity.HasRows = entity.End != null;
        }

        public override Fields GetEntitySchema(Process process, string name, string schema = "", bool isMaster = false) {
            return new Fields();
        }

        private int GetMaxTflBatchId(Entity entity) {
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
            return Convert.ToInt32((result.Response["aggregations"]["tflbatchid"]["value"].Value ?? 0));
        }

        private int GetMaxTflBatchId(string processName) {
            var client = ElasticSearchClientFactory.Create(this, TflBatchEntity(processName));
            var body = new {
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
            return Convert.ToInt32((result.Response["aggregations"]["tflbatchid"]["value"].Value ?? 0));
        }

    }
}