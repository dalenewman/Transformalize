using System;
using System.Collections.Generic;
using System.Web;
using Transformalize.Configuration;
using Transformalize.Libs.Ninject.Modules;
using Transformalize.Libs.Ninject.Syntax;
using Transformalize.Libs.NLog;
using Transformalize.Libs.NLog.Internal;
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
using System.Linq;

namespace Transformalize.Main {

    public class NinjectBindings : NinjectModule {
        private readonly ProcessConfigurationElement _element;
        private const string CORE_ID = "CoreId";
        private readonly Logger _log = LogManager.GetLogger("tfl");
        private readonly Type _type = typeof(Dictionary<string, object>);

        public NinjectBindings(ProcessConfigurationElement element) {
            _element = element;
        }

        public override void Load() {

            // databases
            Bind<AbstractConnectionDependencies>().To<SqlServerDependencies>().WhenInjectedInto<SqlServerConnection>();
            Bind<AbstractConnectionDependencies>().To<MySqlDependencies>().WhenInjectedInto<MySqlConnection>();
            Bind<AbstractConnectionDependencies>().To<PostgreSqlDependencies>().WhenInjectedInto<PostgreSqlConnection>();
            Bind<AbstractConnectionDependencies>().To<SqlCeDependencies>().WhenInjectedInto<SqlCeConnection>();

            Bind<AbstractConnection>().To<SqlServerConnection>().Named("sqlserver");
            Bind<AbstractConnection>().To<MySqlConnection>().Named("mysql");
            Bind<AbstractConnection>().To<PostgreSqlConnection>().Named("postgresql");
            Bind<AbstractConnection>().To<SqlCeConnection>().Named("sqlce");

            // others
            Bind<AbstractConnectionDependencies>().To<AnalysisServicesDependencies>().WhenInjectedInto<AnalysisServicesConnection>();
            Bind<AbstractConnectionDependencies>().To<FileDependencies>().WhenInjectedInto<FileConnection>();
            Bind<AbstractConnectionDependencies>().To<FolderDependencies>().WhenInjectedInto<FolderConnection>();
            Bind<AbstractConnectionDependencies>().To<InternalDependencies>().WhenInjectedInto<InternalConnection>();
            Bind<AbstractConnectionDependencies>().To<ConsoleDependencies>().WhenInjectedInto<ConsoleConnection>();
            Bind<AbstractConnectionDependencies>().To<LogDependencies>().WhenInjectedInto<LogConnection>();
            Bind<AbstractConnectionDependencies>().To<MailDependencies>().WhenInjectedInto<MailConnection>();
            Bind<AbstractConnectionDependencies>().To<HtmlDependencies>().WhenInjectedInto<HtmlConnection>();
            Bind<AbstractConnectionDependencies>().To<ElasticSearchDependencies>().WhenInjectedInto<ElasticSearchConnection>();
            Bind<AbstractConnectionDependencies>().To<SolrDependencies>().WhenInjectedInto<SolrConnection>();
            Bind<AbstractConnectionDependencies>().To<LuceneDependencies>().WhenInjectedInto<LuceneConnection>().WithConstructorArgument("processName", _element.Name);

            Bind<AbstractConnection>().To<AnalysisServicesConnection>().Named("analysisservices");
            Bind<AbstractConnection>().To<FileConnection>().Named("file");
            Bind<AbstractConnection>().To<FolderConnection>().Named("folder");
            Bind<AbstractConnection>().To<InternalConnection>().Named("internal");
            Bind<AbstractConnection>().To<ConsoleConnection>().Named("console");
            Bind<AbstractConnection>().To<LogConnection>().Named("log");
            Bind<AbstractConnection>().To<MailConnection>().Named("mail");
            Bind<AbstractConnection>().To<HtmlConnection>().Named("html");
            Bind<AbstractConnection>().To<ElasticSearchConnection>().Named("elasticsearch");
            Bind<AbstractConnection>().To<SolrConnection>().Named("solr");
            Bind<AbstractConnection>().To<LuceneConnection>().Named("lucene");

            //solrnet binding is a major pain in the ass
            var solrConnections = _element.Connections.Cast<ConnectionConfigurationElement>().Where(c => c.Provider.Equals("solr", StringComparison.OrdinalIgnoreCase)).ToArray();
            if (solrConnections.Any()) {
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
                })
                    Bind<IValidationRule>().To(p);

                Bind(typeof(ISolrMoreLikeThisHandlerQueryResultsParser<>)).To(typeof(SolrMoreLikeThisHandlerQueryResultsParser<>));
                Bind(typeof(ISolrDocumentSerializer<>)).To(typeof(SolrDocumentSerializer<>));
                Bind(typeof(ISolrDocumentSerializer<Dictionary<string, object>>)).To(typeof(SolrDictionarySerializer));

                Bind<ISolrSchemaParser>().To<SolrSchemaParser>();
                Bind<ISolrDIHStatusParser>().To<SolrDIHStatusParser>();
                Bind<IMappingValidator>().To<MappingValidator>();
                Bind<ISolrStatusResponseParser>().To<SolrStatusResponseParser>();
                Bind<ISolrCoreAdmin>().To<SolrCoreAdmin>();

                foreach (EntityConfigurationElement entity in _element.Entities) {
                    foreach (ConnectionConfigurationElement cn in solrConnections) {
                        if (cn.Name.Equals(entity.Connection, StringComparison.OrdinalIgnoreCase)) {
                            var coreUrl = cn.NormalizeUrl(8983) + "/" + (entity.PrependProcessNameToOutputName ? _element.Name + entity.Alias : entity.Alias);
                            _log.Info("Registering SOLR core {0}", coreUrl);

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
                }
            }
        }
    }
}

