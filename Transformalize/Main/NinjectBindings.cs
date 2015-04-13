using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Transformalize.Configuration;
using Transformalize.Libs.Ninject.Modules;
using Transformalize.Libs.Ninject.Syntax;
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
using Transformalize.Logging;
using Transformalize.Main.Providers;
using Transformalize.Main.Providers.AnalysisServices;
using Transformalize.Main.Providers.Console;
using Transformalize.Main.Providers.ElasticSearch;
using Transformalize.Main.Providers.File;
using Transformalize.Main.Providers.Folder;
using Transformalize.Main.Providers.Html;
using Transformalize.Main.Providers.Internal;
using Transformalize.Main.Providers.Log;
using Transformalize.Main.Providers.Lucene;
using Transformalize.Main.Providers.Mail;
using Transformalize.Main.Providers.MySql;
using Transformalize.Main.Providers.PostgreSql;
using Transformalize.Main.Providers.Solr;
using Transformalize.Main.Providers.SqlCe;
using Transformalize.Main.Providers.SqlServer;
using SolrConnection = Transformalize.Main.Providers.Solr.SolrConnection;

namespace Transformalize.Main {

    public class NinjectBindings : NinjectModule {

        private readonly TflProcess _process;
        private const string CORE_ID = "CoreId";
        private readonly Type _type = typeof(Dictionary<string, object>);

        public NinjectBindings(TflProcess process) {
            _process = process;
        }

        public override void Load() {

            Bind<AbstractConnectionDependencies>().To<InternalDependencies>().WhenInjectedInto<InternalConnection>();
            Bind<AbstractConnection>().To<InternalConnection>().Named("internal");

            var providers = _process
                .Connections
                .Where(c => c.Provider != "internal")
                .Select(c => c.Provider)
                .Distinct();

            foreach (var provider in providers) {
                switch (provider) {
                    case "sqlserver":
                        Bind<AbstractConnectionDependencies>().To<SqlServerDependencies>().WhenInjectedInto<SqlServerConnection>();
                        Bind<AbstractConnection>().To<SqlServerConnection>().Named(provider);
                        break;
                    case "mysql":
                        Bind<AbstractConnectionDependencies>().To<MySqlDependencies>().WhenInjectedInto<MySqlConnection>();
                        Bind<AbstractConnection>().To<MySqlConnection>().Named(provider);
                        break;
                    case "postgresql":
                        Bind<AbstractConnectionDependencies>().To<PostgreSqlDependencies>().WhenInjectedInto<PostgreSqlConnection>();
                        Bind<AbstractConnection>().To<PostgreSqlConnection>().Named(provider);
                        break;
                    case "sqlce":
                        Bind<AbstractConnectionDependencies>().To<SqlCeDependencies>().WhenInjectedInto<SqlCeConnection>();
                        Bind<AbstractConnection>().To<SqlCeConnection>().Named(provider);
                        break;
                    case "analysisservices":
                        Bind<AbstractConnectionDependencies>().To<AnalysisServicesDependencies>().WhenInjectedInto<AnalysisServicesConnection>();
                        Bind<AbstractConnection>().To<AnalysisServicesConnection>().Named(provider);
                        break;
                    case "file":
                        Bind<AbstractConnectionDependencies>().To<FileDependencies>().WhenInjectedInto<FileConnection>();
                        Bind<AbstractConnection>().To<FileConnection>().Named(provider);
                        break;
                    case "folder":
                        Bind<AbstractConnectionDependencies>().To<FolderDependencies>().WhenInjectedInto<FolderConnection>();
                        Bind<AbstractConnection>().To<FolderConnection>().Named("folder");
                        break;
                    case "console":
                        Bind<AbstractConnectionDependencies>().To<ConsoleDependencies>().WhenInjectedInto<ConsoleConnection>();
                        Bind<AbstractConnection>().To<ConsoleConnection>().Named(provider);
                        break;
                    case "log":
                        Bind<AbstractConnectionDependencies>().To<LogDependencies>().WhenInjectedInto<LogConnection>();
                        Bind<AbstractConnection>().To<LogConnection>().Named(provider);
                        break;
                    case "mail":
                        Bind<AbstractConnectionDependencies>().To<MailDependencies>().WhenInjectedInto<MailConnection>();
                        Bind<AbstractConnection>().To<MailConnection>().Named(provider);
                        break;
                    case "html":
                        Bind<AbstractConnectionDependencies>().To<HtmlDependencies>().WhenInjectedInto<HtmlConnection>();
                        Bind<AbstractConnection>().To<HtmlConnection>().Named(provider);
                        break;
                    case "elasticsearch":
                        Bind<AbstractConnectionDependencies>().To<ElasticSearchDependencies>().WhenInjectedInto<ElasticSearchConnection>();
                        Bind<AbstractConnection>().To<ElasticSearchConnection>().Named(provider);
                        break;
                    case "solr":
                        Bind<AbstractConnectionDependencies>().To<SolrDependencies>().WhenInjectedInto<SolrConnection>();
                        Bind<AbstractConnection>().To<SolrConnection>().Named(provider);

                        //solrnet binding
                        var mapper = new MemoizingMappingManager(new AttributesMappingManager());
                        Bind<IReadOnlyMappingManager>().ToConstant(mapper);
                        if (HttpRuntime.AppDomainAppId != null) {
                            Bind<ISolrCache>().To<HttpRuntimeCache>();
                        }
                        Bind<ISolrDocumentPropertyVisitor>().To<DefaultDocumentVisitor>();
                        Bind<ISolrFieldParser>().To<DefaultFieldParser>();
                        Bind(typeof(ISolrDocumentActivator<>)).To(typeof(SolrDocumentActivator<>));
                        Bind(typeof(ISolrDocumentResponseParser<>)).To(typeof(SolrDocumentResponseParser<>));
                        Bind<ISolrDocumentResponseParser<Dictionary<string, object>>>().To<SolrDictionaryDocumentResponseParser>();
                        Bind<ISolrFieldSerializer>().To<DefaultFieldSerializer>();
                        Bind<ISolrQuerySerializer>().To<DefaultQuerySerializer>();
                        Bind<ISolrFacetQuerySerializer>().To<DefaultFacetQuerySerializer>();
                        Bind(typeof(ISolrAbstractResponseParser<>)).To(typeof(DefaultResponseParser<>));
                        Bind<ISolrHeaderResponseParser>().To<HeaderResponseParser<string>>();
                        Bind<ISolrExtractResponseParser>().To<ExtractResponseParser>();

                        foreach (var p in new[] {
                            typeof(MappedPropertiesIsInSolrSchemaRule),
                            typeof(RequiredFieldsAreMappedRule),
                            typeof(UniqueKeyMatchesMappingRule),
                            typeof(MultivaluedMappedToCollectionRule),
                        }) {
                            Bind<IValidationRule>().To(p);
                        }

                        Bind(typeof(ISolrMoreLikeThisHandlerQueryResultsParser<>)).To(typeof(SolrMoreLikeThisHandlerQueryResultsParser<>));
                        Bind(typeof(ISolrDocumentSerializer<>)).To(typeof(SolrDocumentSerializer<>));
                        Bind(typeof(ISolrDocumentSerializer<Dictionary<string, object>>)).To(typeof(SolrDictionarySerializer));

                        Bind<ISolrSchemaParser>().To<SolrSchemaParser>();
                        Bind<ISolrDIHStatusParser>().To<SolrDIHStatusParser>();
                        Bind<IMappingValidator>().To<MappingValidator>();
                        Bind<ISolrStatusResponseParser>().To<SolrStatusResponseParser>();
                        Bind<ISolrCoreAdmin>().To<SolrCoreAdmin>();

                        // each entity-core must be bound
                        foreach (TflEntity entity in _process.Entities) {
                            var connection = entity.Connection;

                            foreach (TflConnection cn in _process.Connections.Where(c => c.Name == connection && c.Provider == "solr")) {

                                var coreUrl = cn.NormalizeUrl(8983) + "/" + (entity.PrependProcessNameToOutputName ? _process.Name + entity.Alias : entity.Alias);
                                TflLogger.Info(_process.Name, entity.Name, "Registering SOLR core {0}", coreUrl);

                                Bind<ISolrConnection>().ToConstant(new Libs.SolrNet.Impl.SolrConnection(coreUrl))
                                    .WithMetadata(CORE_ID, coreUrl);

                                var iSolrQueryExecuter = typeof(ISolrQueryExecuter<>).MakeGenericType(_type);
                                var solrQueryExecuter = typeof(SolrQueryExecuter<>).MakeGenericType(_type);

                                Bind(iSolrQueryExecuter).To(solrQueryExecuter)
                                    .Named(coreUrl + solrQueryExecuter)
                                    .WithMetadata(CORE_ID, coreUrl)
                                    .WithConstructorArgument("connection", ctx => ctx.Kernel.Get<ISolrConnection>(bindingMetaData => bindingMetaData.Has(CORE_ID) && bindingMetaData.Get<string>(CORE_ID).Equals(coreUrl)));

                                var solrBasicOperations = typeof(ISolrBasicOperations<>).MakeGenericType(_type);
                                var solrBasicReadOnlyOperations = typeof(ISolrBasicReadOnlyOperations<>).MakeGenericType(_type);
                                var solrBasicServer = typeof(SolrBasicServer<>).MakeGenericType(_type);

                                Bind(solrBasicOperations).To(solrBasicServer)
                                    .Named(coreUrl + solrBasicServer)
                                    .WithMetadata(CORE_ID, coreUrl)
                                    .WithConstructorArgument("connection", ctx => ctx.Kernel.Get<ISolrConnection>(bindingMetaData => bindingMetaData.Has(CORE_ID) && bindingMetaData.Get<string>(CORE_ID).Equals(coreUrl)))
                                    .WithConstructorArgument("queryExecuter", ctx => ctx.Kernel.Get(iSolrQueryExecuter, bindingMetaData => bindingMetaData.Has(CORE_ID) && bindingMetaData.Get<string>(CORE_ID).Equals(coreUrl)));

                                Bind(solrBasicReadOnlyOperations).To(solrBasicServer)
                                    .Named(coreUrl + solrBasicServer)
                                    .WithMetadata(CORE_ID, coreUrl)
                                    .WithConstructorArgument("connection", ctx => ctx.Kernel.Get<ISolrConnection>(bindingMetaData => bindingMetaData.Has(CORE_ID) && bindingMetaData.Get<string>(CORE_ID).Equals(coreUrl)))
                                    .WithConstructorArgument("queryExecuter", ctx => ctx.Kernel.Get(iSolrQueryExecuter, bindingMetaData => bindingMetaData.Has(CORE_ID) && bindingMetaData.Get<string>(CORE_ID).Equals(coreUrl)));

                                var solrOperations = typeof(ISolrOperations<>).MakeGenericType(_type);
                                var solrServer = typeof(SolrServer<>).MakeGenericType(_type);
                                var solrReadOnlyOperations = typeof(ISolrReadOnlyOperations<>).MakeGenericType(_type);

                                Bind(solrOperations).To(solrServer)
                                    .Named(coreUrl)
                                    .WithMetadata(CORE_ID, coreUrl)
                                    .WithConstructorArgument("basicServer", ctx => ctx.Kernel.Get(solrBasicOperations, bindingMetaData => bindingMetaData.Has(CORE_ID) && bindingMetaData.Get<string>(CORE_ID).Equals(coreUrl)));

                                Bind(solrReadOnlyOperations).To(solrServer)
                                    .Named(coreUrl)
                                    .WithMetadata(CORE_ID, coreUrl)
                                    .WithConstructorArgument("basicServer", ctx => ctx.Kernel.Get(solrBasicReadOnlyOperations, bindingMetaData => bindingMetaData.Has(CORE_ID) && bindingMetaData.Get<string>(CORE_ID).Equals(coreUrl)));
                            }
                        }
                        break;
                    case "lucene":
                        Bind<AbstractConnectionDependencies>().To<LuceneDependencies>().WhenInjectedInto<LuceneConnection>().WithConstructorArgument("processName", _process.Name);
                        Bind<AbstractConnection>().To<LuceneConnection>().Named(provider);
                        break;
                    case "web":
                        Bind<AbstractConnectionDependencies>().To<WebDependencies>().WhenInjectedInto<WebConnection>();
                        Bind<AbstractConnection>().To<WebConnection>().Named(provider);
                        break;
                }
            }

        }
    }
}

