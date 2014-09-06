using System.Collections.Generic;
using Transformalize.Libs.Ninject.Modules;
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

namespace Transformalize.Main {

    public class NinjectBindings : NinjectModule {
        private readonly string _processName;

        public NinjectBindings(string processName)
        {
            _processName = processName;
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
            Bind<AbstractConnectionDependencies>().To<SolrDependencies>().WhenInjectedInto<Providers.Solr.SolrConnection>();
            Bind<AbstractConnectionDependencies>().To<LuceneDependencies>().WhenInjectedInto<LuceneConnection>().WithConstructorArgument("processName", _processName);

            Bind<AbstractConnection>().To<AnalysisServicesConnection>().Named("analysisservices");
            Bind<AbstractConnection>().To<FileConnection>().Named("file");
            Bind<AbstractConnection>().To<FolderConnection>().Named("folder");
            Bind<AbstractConnection>().To<InternalConnection>().Named("internal");
            Bind<AbstractConnection>().To<ConsoleConnection>().Named("console");
            Bind<AbstractConnection>().To<LogConnection>().Named("log");
            Bind<AbstractConnection>().To<MailConnection>().Named("mail");
            Bind<AbstractConnection>().To<HtmlConnection>().Named("html");
            Bind<AbstractConnection>().To<ElasticSearchConnection>().Named("elasticsearch");
            Bind<AbstractConnection>().To<Providers.Solr.SolrConnection>().Named("solr");
            Bind<AbstractConnection>().To<LuceneConnection>().Named("lucene");

        }

    }
}
