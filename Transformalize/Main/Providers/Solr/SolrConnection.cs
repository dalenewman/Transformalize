using System;
using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Libs.Ninject.Syntax;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Libs.SolrNet;
using Transformalize.Libs.SolrNet.Impl;
using Transformalize.Operations.Transform;

namespace Transformalize.Main.Providers.Solr {

    public class SolrConnection : AbstractConnection {
        private const string CORE_ID = "CoreId";
        private readonly Process _process;
        private readonly string _coreUrl;
        private readonly Type _type = typeof(Dictionary<string, object>);

        public override string UserProperty { get { return string.Empty; } }
        public override string PasswordProperty { get { return string.Empty; } }
        public override string PortProperty { get { return string.Empty; } }
        public override string DatabaseProperty { get { return string.Empty; } }
        public override string ServerProperty { get { return string.Empty; } }
        public override string TrustedProperty { get { return string.Empty; } }
        public override string PersistSecurityInfoProperty { get { return string.Empty; } }
        public string CoreUrl { get { return _coreUrl; } }

        public SolrConnection(Process process, ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            _process = process;
            TypeAndAssemblyName = process.Providers[element.Provider.ToLower()];
            Type = ProviderType.Solr;
            IsDatabase = true;

            var builder = new UriBuilder(element.Server);
            if (element.Port > 0) {
                builder.Port = element.Port;
            }
            builder.Path = element.Path;
            _coreUrl = builder.ToString();

            process.Kernal.Bind<ISolrConnection>().ToConstant(new Libs.SolrNet.Impl.SolrConnection(_coreUrl))
              .WithMetadata(CORE_ID, _coreUrl);

            var iSolrQueryExecuter = typeof(ISolrQueryExecuter<>).MakeGenericType(_type);
            var solrQueryExecuter = typeof(SolrQueryExecuter<>).MakeGenericType(_type);

            process.Kernal.Bind(iSolrQueryExecuter).To(solrQueryExecuter)
                .Named(_coreUrl + solrQueryExecuter)
                .WithMetadata(CORE_ID, _coreUrl)
                .WithConstructorArgument("connection", ctx => ctx.Kernel.Get<ISolrConnection>(bindingMetaData => bindingMetaData.Has(CORE_ID) && bindingMetaData.Get<string>(CORE_ID).Equals(_coreUrl)));

            var solrBasicOperations = typeof(ISolrBasicOperations<>).MakeGenericType(_type);
            var solrBasicReadOnlyOperations = typeof(ISolrBasicReadOnlyOperations<>).MakeGenericType(_type);
            var solrBasicServer = typeof(SolrBasicServer<>).MakeGenericType(_type);

            process.Kernal.Bind(solrBasicOperations).To(solrBasicServer)
                .Named(_coreUrl + solrBasicServer)
                .WithMetadata(CORE_ID, _coreUrl)
                .WithConstructorArgument("connection", ctx => ctx.Kernel.Get<ISolrConnection>(bindingMetaData => bindingMetaData.Has(CORE_ID) && bindingMetaData.Get<string>(CORE_ID).Equals(_coreUrl)))
                .WithConstructorArgument("queryExecuter", ctx => ctx.Kernel.Get(iSolrQueryExecuter, bindingMetaData => bindingMetaData.Has(CORE_ID) && bindingMetaData.Get<string>(CORE_ID).Equals(_coreUrl)));

            process.Kernal.Bind(solrBasicReadOnlyOperations).To(solrBasicServer)
                .Named(_coreUrl + solrBasicServer)
                .WithMetadata(CORE_ID, _coreUrl)
                .WithConstructorArgument("connection", ctx => ctx.Kernel.Get<ISolrConnection>(bindingMetaData => bindingMetaData.Has(CORE_ID) && bindingMetaData.Get<string>(CORE_ID).Equals(_coreUrl)))
                .WithConstructorArgument("queryExecuter", ctx => ctx.Kernel.Get(iSolrQueryExecuter, bindingMetaData => bindingMetaData.Has(CORE_ID) && bindingMetaData.Get<string>(CORE_ID).Equals(_coreUrl)));

            var solrOperations = typeof(ISolrOperations<>).MakeGenericType(_type);
            var solrServer = typeof(SolrServer<>).MakeGenericType(_type);
            var solrReadOnlyOperations = typeof(ISolrReadOnlyOperations<>).MakeGenericType(_type);

            process.Kernal.Bind(solrOperations).To(solrServer)
                .Named(_coreUrl)
                .WithMetadata(CORE_ID, _coreUrl)
                .WithConstructorArgument("basicServer", ctx => ctx.Kernel.Get(solrBasicOperations, bindingMetaData => bindingMetaData.Has(CORE_ID) && bindingMetaData.Get<string>(CORE_ID).Equals(_coreUrl)));
            process.Kernal.Bind(solrReadOnlyOperations).To(solrServer)
                .Named(_coreUrl)
                .WithMetadata(CORE_ID, _coreUrl)
                .WithConstructorArgument("basicServer", ctx => ctx.Kernel.Get(solrBasicReadOnlyOperations, bindingMetaData => bindingMetaData.Has(CORE_ID) && bindingMetaData.Get<string>(CORE_ID).Equals(_coreUrl)));
        }

        public override int NextBatchId(string processName) {
            if (!TflBatchRecordsExist(processName)) {
                return 1;
            }
            return GetMaxTflBatchId(processName) + 1;
        }

        public override void WriteEndVersion(AbstractConnection input, Entity entity) {
            if (entity.Inserts + entity.Updates > 0 || _process.IsFirstRun) {

                var solr = _process.Kernal.Get<ISolrOperations<Dictionary<string, object>>>(_coreUrl);
                var versionType = entity.Version == null ? "string" : entity.Version.SimpleType;
                var end = versionType.Equals("byte[]") || versionType.Equals("rowversion") ? Common.BytesToHexString((byte[])entity.End) : new DefaultFactory().Convert(entity.End, versionType).ToString();

                var doc = new Dictionary<string,object> {
                    { "id", entity.TflBatchId},
                    { "tflbatchid", entity.TflBatchId},
                    { "process", entity.ProcessName},
                    { "connection", input.Name},
                    { "entity", entity.Alias},
                    { "updates", entity.Updates},
                    { "inserts", entity.Inserts},
                    { "deletes", entity.Deletes},
                    { "version", end},
                    { "version_type", versionType},
                    { "tflupdate", DateTime.UtcNow}
                };
                solr.Add(doc);
                solr.Commit();
            }
        }

        public override IOperation EntityOutputKeysExtract(Entity entity) {
            return new EmptyOperation();
            //need to learn search type scan / scroll functionality for bigger result sets
            //return new SolrEntityOutputKeysExtract(this, entity);
        }

        public override IOperation EntityOutputKeysExtractAll(Entity entity) {
            return new SolrEntityOutputKeysExtract(this, entity);
        }

        public override IOperation EntityBulkLoad(Entity entity) {
            return new SolrLoadOperation(entity, this);
        }

        public override IOperation EntityBatchUpdate(Entity entity) {
            return new SolrLoadOperation(entity, this);
        }

        public override void LoadBeginVersion(Entity entity) {
            var tflBatchId = GetMaxTflBatchId(entity);
            if (tflBatchId > 0) {
                //entity.Begin = Common.GetObjectConversionMap()[versionType](hits[0]["_source"]["version"].Value);
                entity.HasRange = true;
            }
        }

        public override void LoadEndVersion(Entity entity) {

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
            //var result = client.Client.Search(client.Index, client.Type, body);
            //entity.End = Common.GetObjectConversionMap()[entity.Version.SimpleType](result.Response["aggregations"]["version"]["value"].Value);
            entity.HasRows = entity.End != null;
        }

        private int GetMaxTflBatchId(Entity entity) {
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
            //var result = client.Client.Search(client.Index, client.Type, body);
            //return Convert.ToInt32((result.Response["aggregations"]["tflbatchid"]["value"].Value ?? 0));
            throw new NotImplementedException();
        }

        private int GetMaxTflBatchId(string processName) {

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
            //var result = client.Client.Search(client.Index, client.Type, body);
            //return Convert.ToInt32((result.Response["aggregations"]["tflbatchid"]["value"].Value ?? 0));
            throw new NotImplementedException();
        }

    }
}