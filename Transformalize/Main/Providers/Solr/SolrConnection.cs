using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Libs.Ninject;
using Transformalize.Libs.Ninject.Syntax;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Libs.SolrNet;
using Transformalize.Libs.SolrNet.Impl;
using Transformalize.Libs.SolrNet.Impl.DocumentPropertyVisitors;
using Transformalize.Libs.SolrNet.Impl.FacetQuerySerializers;
using Transformalize.Libs.SolrNet.Impl.FieldParsers;
using Transformalize.Libs.SolrNet.Impl.FieldSerializers;
using Transformalize.Libs.SolrNet.Impl.QuerySerializers;
using Transformalize.Libs.SolrNet.Impl.ResponseParsers;
using Transformalize.Libs.SolrNet.Mapping;
using Transformalize.Libs.SolrNet.Mapping.Validation;
using Transformalize.Libs.SolrNet.Mapping.Validation.Rules;
using Transformalize.Libs.SolrNet.Schema;
using Transformalize.Operations.Transform;

namespace Transformalize.Main.Providers.Solr {

    public class SolrConnection : AbstractConnection {

        private const string CORE_ID = "CoreId";
        private const int DEFAULT_PORT = 8983;
        private readonly char[] _slash = { '/' };
        private readonly string _solrUrl;
        private readonly string _schemaFile = "schema.xml";
        private readonly Type _type = typeof(Dictionary<string, object>);
        private readonly IKernel _kernal = new StandardKernel();
        private readonly ConcurrentDictionary<string, byte> _registeredCores = new ConcurrentDictionary<string, byte>();

        public static Dictionary<string, string> SolrTypeMap = new Dictionary<string, string>() {
            {"text_en_splitting","string"}
        };

        public SolrConnection(ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            Type = ProviderType.Solr;
            IsDatabase = true;
            _solrUrl = NormalizeSolrUrl(element);

            if (!element.File.Equals(string.Empty)) {
                _schemaFile = element.File;
            }

            var mapper = new MemoizingMappingManager(new AttributesMappingManager());
            _kernal.Bind<IReadOnlyMappingManager>().ToConstant(mapper);
            //_kernal.Bind<ISolrCache>().To<HttpRuntimeCache>();
            _kernal.Bind<ISolrDocumentPropertyVisitor>().To<DefaultDocumentVisitor>();
            _kernal.Bind<ISolrFieldParser>().To<DefaultFieldParser>();
            _kernal.Bind(typeof(ISolrDocumentActivator<>)).To(typeof(SolrDocumentActivator<>));
            _kernal.Bind(typeof(ISolrDocumentResponseParser<>)).To(typeof(SolrDocumentResponseParser<>));
            _kernal.Bind<ISolrDocumentResponseParser<Dictionary<string, object>>>().To<SolrDictionaryDocumentResponseParser>();
            _kernal.Bind<ISolrFieldSerializer>().To<DefaultFieldSerializer>();
            _kernal.Bind<ISolrQuerySerializer>().To<DefaultQuerySerializer>();
            _kernal.Bind<ISolrFacetQuerySerializer>().To<DefaultFacetQuerySerializer>();
            _kernal.Bind(typeof(ISolrAbstractResponseParser<>)).To(typeof(DefaultResponseParser<>));
            _kernal.Bind<ISolrHeaderResponseParser>().To<HeaderResponseParser<string>>();
            _kernal.Bind<ISolrExtractResponseParser>().To<ExtractResponseParser>();

            foreach (var p in new[] {
                typeof(MappedPropertiesIsInSolrSchemaRule),
                typeof(RequiredFieldsAreMappedRule),
                typeof(UniqueKeyMatchesMappingRule),
                typeof(MultivaluedMappedToCollectionRule),
            })
                _kernal.Bind<IValidationRule>().To(p);

            _kernal.Bind(typeof(ISolrMoreLikeThisHandlerQueryResultsParser<>)).To(typeof(SolrMoreLikeThisHandlerQueryResultsParser<>));
            _kernal.Bind(typeof(ISolrDocumentSerializer<>)).To(typeof(SolrDocumentSerializer<>));
            _kernal.Bind(typeof(ISolrDocumentSerializer<Dictionary<string, object>>)).To(typeof(SolrDictionarySerializer));

            _kernal.Bind<ISolrSchemaParser>().To<SolrSchemaParser>();
            _kernal.Bind<ISolrDIHStatusParser>().To<SolrDIHStatusParser>();
            _kernal.Bind<IMappingValidator>().To<MappingValidator>();
            _kernal.Bind<ISolrStatusResponseParser>().To<SolrStatusResponseParser>();
            _kernal.Bind<ISolrCoreAdmin>().To<SolrCoreAdmin>();

        }

        public string NormalizeSolrUrl(ConnectionConfigurationElement element) {
            var builder = new UriBuilder(element.Server);
            if (element.Port > 0) {
                builder.Port = element.Port;
            }
            if (builder.Port == 0) {
                builder.Port = DEFAULT_PORT;
            }
            if (!element.Path.Equals(string.Empty) && element.Path != builder.Path) {
                builder.Path = builder.Path.TrimEnd(_slash) + "/" + element.Path.TrimStart(_slash);
            } else if (!element.Folder.Equals(string.Empty) && element.Folder != builder.Path) {
                builder.Path = builder.Path.TrimEnd(_slash) + "/" + element.Folder.TrimStart(_slash);
            }
            return builder.ToString();
        }

        public string GetPingUrl() {
            return _solrUrl.TrimEnd(_slash) + "/admin/ping";
        }

        public string GetCoreUrl(string entityName) {
            return _solrUrl.TrimEnd(_slash) + "/" + entityName.TrimStart(_slash);
        }

        public void RegisterCore(string entityName) {

            var coreUrl = GetCoreUrl(entityName);
            if (_registeredCores.ContainsKey(coreUrl))
                return;

            _kernal.Bind<ISolrConnection>().ToConstant(new Libs.SolrNet.Impl.SolrConnection(coreUrl))
              .WithMetadata(CORE_ID, coreUrl);

            var iSolrQueryExecuter = typeof(ISolrQueryExecuter<>).MakeGenericType(_type);
            var solrQueryExecuter = typeof(SolrQueryExecuter<>).MakeGenericType(_type);

            _kernal.Bind(iSolrQueryExecuter).To(solrQueryExecuter)
                .Named(coreUrl + solrQueryExecuter)
                .WithMetadata(CORE_ID, coreUrl)
                .WithConstructorArgument("connection", ctx => ctx.Kernel.Get<ISolrConnection>(bindingMetaData => bindingMetaData.Has(CORE_ID) && bindingMetaData.Get<string>(CORE_ID).Equals(coreUrl)));

            var solrBasicOperations = typeof(ISolrBasicOperations<>).MakeGenericType(_type);
            var solrBasicReadOnlyOperations = typeof(ISolrBasicReadOnlyOperations<>).MakeGenericType(_type);
            var solrBasicServer = typeof(SolrBasicServer<>).MakeGenericType(_type);

            _kernal.Bind(solrBasicOperations).To(solrBasicServer)
                .Named(coreUrl + solrBasicServer)
                .WithMetadata(CORE_ID, coreUrl)
                .WithConstructorArgument("connection", ctx => ctx.Kernel.Get<ISolrConnection>(bindingMetaData => bindingMetaData.Has(CORE_ID) && bindingMetaData.Get<string>(CORE_ID).Equals(coreUrl)))
                .WithConstructorArgument("queryExecuter", ctx => ctx.Kernel.Get(iSolrQueryExecuter, bindingMetaData => bindingMetaData.Has(CORE_ID) && bindingMetaData.Get<string>(CORE_ID).Equals(coreUrl)));

            _kernal.Bind(solrBasicReadOnlyOperations).To(solrBasicServer)
                .Named(coreUrl + solrBasicServer)
                .WithMetadata(CORE_ID, coreUrl)
                .WithConstructorArgument("connection", ctx => ctx.Kernel.Get<ISolrConnection>(bindingMetaData => bindingMetaData.Has(CORE_ID) && bindingMetaData.Get<string>(CORE_ID).Equals(coreUrl)))
                .WithConstructorArgument("queryExecuter", ctx => ctx.Kernel.Get(iSolrQueryExecuter, bindingMetaData => bindingMetaData.Has(CORE_ID) && bindingMetaData.Get<string>(CORE_ID).Equals(coreUrl)));

            var solrOperations = typeof(ISolrOperations<>).MakeGenericType(_type);
            var solrServer = typeof(SolrServer<>).MakeGenericType(_type);
            var solrReadOnlyOperations = typeof(ISolrReadOnlyOperations<>).MakeGenericType(_type);

            _kernal.Bind(solrOperations).To(solrServer)
                .Named(coreUrl)
                .WithMetadata(CORE_ID, coreUrl)
                .WithConstructorArgument("basicServer", ctx => ctx.Kernel.Get(solrBasicOperations, bindingMetaData => bindingMetaData.Has(CORE_ID) && bindingMetaData.Get<string>(CORE_ID).Equals(coreUrl)));
            _kernal.Bind(solrReadOnlyOperations).To(solrServer)
                .Named(coreUrl)
                .WithMetadata(CORE_ID, coreUrl)
                .WithConstructorArgument("basicServer", ctx => ctx.Kernel.Get(solrBasicReadOnlyOperations, bindingMetaData => bindingMetaData.Has(CORE_ID) && bindingMetaData.Get<string>(CORE_ID).Equals(coreUrl)));

            _registeredCores[coreUrl] = 1;
        }

        public override int NextBatchId(string processName) {
            if (!TflBatchRecordsExist(processName)) {
                return 1;
            }
            return GetMaxTflBatchId(processName) + 1;
        }

        public override void WriteEndVersion(AbstractConnection input, Entity entity, bool force = false) {

            if (entity.Updates + entity.Inserts > 0 || force) {

                var solr = _kernal.Get<ISolrOperations<Dictionary<string, object>>>(_solrUrl);
                var versionType = entity.Version == null ? "string" : entity.Version.SimpleType;
                var end = versionType.Equals("byte[]") || versionType.Equals("rowversion") ? Common.BytesToHexString((byte[])entity.End) : new DefaultFactory().Convert(entity.End, versionType).ToString();

                var doc = new Dictionary<string, object> {
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

        public override IOperation ExtractCorrespondingKeysFromOutput(Entity entity) {
            return new EmptyOperation();
            //need to learn search type scan / scroll functionality for bigger result sets
            //return new SolrEntityOutputKeysExtract(this, entity);
        }

        public override IOperation ExtractAllKeysFromOutput(Entity entity) {
            return new SolrEntityOutputKeysExtract(this, entity);
        }

        public override IOperation ExtractAllKeysFromInput(Entity entity) {
            return new EmptyOperation();
        }

        public override IOperation Insert(Entity entity) {
            return new SolrLoadOperation(entity, this);
        }

        public override IOperation Update(Entity entity) {
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

        public override Fields GetEntitySchema(Process process, string name, string schema = "", bool isMaster = false) {
            var fields = new Fields();
            var coreUrl = GetCoreUrl(name);
            RegisterCore(name);
            var solr = _kernal.Get<ISolrReadOnlyOperations<Dictionary<string, object>>>(coreUrl);
            var solrSchema = solr.GetSchema(_schemaFile);

            foreach (var solrField in solrSchema.SolrFields) {
                string type;
                var searchType = "default";
                if (SolrTypeMap.ContainsKey(solrField.Type.Name)) {
                    type = SolrTypeMap[solrField.Type.Name];
                    searchType = solrField.Type.Name;
                } else {
                    type = solrField.Type.Name;
                }

                var field = new Field(type, "64", FieldType.None, true, string.Empty) {
                    Name = solrField.Name,
                    Entity = name,
                    Input = solrField.IsStored,
                    SearchTypes = new List<SearchType>() { new SearchType() { Name = searchType, Analyzer = searchType } }
                };
                fields.Add(field);
            }
            return fields;
        }

        public override IOperation Delete(Entity entity) {
            throw new NotImplementedException();
        }

        public override IOperation Extract(Entity entity, bool firstRun) {
            throw new NotImplementedException();
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