using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Libs.Jint.Parser.Ast;
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

        public override void WriteEndVersion(Process process, AbstractConnection input, Entity entity, bool force = false) {
            if (entity.Updates + entity.Inserts > 0 || force) {
                var client = new ElasticSearchClientFactory().Create(this, TflBatchEntity(entity.ProcessName));
                var versionType = entity.Version == null ? "string" : entity.Version.SimpleType;

                string end;
                if (versionType.Equals("datetime") && entity.End is DateTime) {
                    end = ((DateTime)entity.End).ToString("yyyy-MM-ddTHH:mm:ss.fff");
                } else if (versionType.Equals("byte[]") || versionType.Equals("rowversion")) {
                    end = Common.BytesToHexString((byte[])entity.End);
                } else {
                    end = new DefaultFactory().Convert(entity.End, versionType).ToString();
                }

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

        public override IOperation ExtractCorrespondingKeysFromOutput(Entity entity) {
            return new EmptyOperation();
        }

        public override IOperation ExtractAllKeysFromOutput(Entity entity) {
            return new EmptyOperation();
        }

        public override IOperation ExtractAllKeysFromInput(Process process, Entity entity) {
            return new ElasticSearchEntityExtract(this, entity, entity.PrimaryKey.Aliases().ToArray());
        }

        public override IOperation Insert(Process process, Entity entity) {
            return new ElasticSearchLoadOperation(entity, this);
        }

        public override IOperation Update(Entity entity) {
            return new ElasticSearchLoadOperation(entity, this);
        }

        public override void LoadBeginVersion(Entity entity) {
            var tflBatchId = GetMaxTflBatchId(entity);
            if (tflBatchId > 0) {
                var client = new ElasticSearchClientFactory().Create(this, TflBatchEntity(entity.ProcessName));
                var result = client.Client.SearchGet(client.Index, client.Type, s => s
                    .AddQueryString("q", "tflbatchid:" + tflBatchId)
                    .AddQueryString("_source_include", "version,version_type")
                    .AddQueryString("size", 1)
                );
                var hits = result.Response["hits"].hits;
                var versionType = (string)hits[0]["_source"]["version_type"].Value;
                entity.Begin = Common.GetObjectConversionMap()[versionType](hits[0]["_source"]["version"].Value);
                entity.HasRange = true;
            }
        }

        public override void LoadEndVersion(Entity entity) {

            var client = new ElasticSearchClientFactory().Create(this, entity);
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

        public override Fields GetEntitySchema(Process process, Entity entity, bool isMaster = false) {
            var client = new ElasticSearchClientFactory().CreateNest(this, entity);
            var mapping = client.Client.GetMapping<dynamic>(m => m
                .Index(client.Index)
                .Type(client.Type)
            );
            if (!mapping.IsValid) {
                throw new TransformalizeException("Trouble getting mapping for {0}:{1} {2}", client.Index, client.Type, mapping.ServerError.Error);
            }
            var fields = new Fields();
            foreach (var pair in mapping.Mapping.Properties) {
                fields.Add(new Field(pair.Value.Type.Name, "64", FieldType.None, true, "") { Name = pair.Key.Name });
            }
            return fields;
        }

        public override IOperation Delete(Entity entity) {
            throw new NotImplementedException();
        }

        public override IOperation Extract(Process process, Entity entity, bool firstRun) {
            return new ElasticSearchEntityExtract(this, entity, entity.OutputFields().Aliases().ToArray());
        }

        private int GetMaxTflBatchId(Entity entity) {
            var client = new ElasticSearchClientFactory().Create(this, TflBatchEntity(entity.ProcessName));
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
            var client = new ElasticSearchClientFactory().Create(this, TflBatchEntity(processName));
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