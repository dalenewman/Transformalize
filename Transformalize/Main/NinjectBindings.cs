using Transformalize.Main.Providers;
using Transformalize.Main.Providers.AnalysisServices;
using Transformalize.Main.Providers.File;
using Transformalize.Main.Providers.Folder;
using Transformalize.Main.Providers.Internal;
using Transformalize.Main.Providers.MySql;
using Transformalize.Main.Providers.SqlServer;

namespace Transformalize.Main {

    public class NinjectBindings : Libs.Ninject.Modules.NinjectModule {

        public override void Load() {

            // MySql
            Bind<AbstractConnectionDependencies>().To<MySqlDependencies>().WhenInjectedInto<MySqlConnection>();
            Bind<AbstractProvider>().To<MySqlProvider>().WhenInjectedInto<MySqlDependencies>();
            Bind<IConnectionChecker>().To<DefaultConnectionChecker>().WhenInjectedInto<MySqlDependencies>();
            Bind<IScriptRunner>().To<DefaultScriptRunner>().WhenInjectedInto<MySqlDependencies>();
            Bind<IProviderSupportsModifier>().To<FalseProviderSupportsModifier>().WhenInjectedInto<MySqlDependencies>();
            Bind<IEntityRecordsExist>().To<MySqlEntityRecordsExist>().WhenInjectedInto<MySqlDependencies>();
            Bind<ITflWriter>().To<MySqlTflWriter>().WhenInjectedInto<MySqlDependencies>();
            Bind<IViewWriter>().To<MySqlViewWriter>().WhenInjectedInto<MySqlDependencies>();
            Bind<ITableQueryWriter>().To<MySqlTableQueryWriter>().WhenInjectedInto<MySqlDependencies>();
            Bind<IEntityDropper>().To<MySqlEntityDropper>().WhenInjectedInto<MySqlDependencies>();
            Bind<IEntityExists>().To<MySqlEntityExists>().WhenInjectedInto<MySqlEntityDropper>();

            // SqlServer
            Bind<AbstractConnectionDependencies>().To<SqlServerDependencies>().WhenInjectedInto<SqlServerConnection>();
            Bind<AbstractProvider>().To<SqlServerProvider>().WhenInjectedInto<SqlServerDependencies>();
            Bind<IConnectionChecker>().To<DefaultConnectionChecker>().WhenInjectedInto<SqlServerDependencies>();
            Bind<IScriptRunner>().To<DefaultScriptRunner>().WhenInjectedInto<SqlServerDependencies>();
            Bind<IProviderSupportsModifier>().To<SqlServerProviderSupportsModifier>().WhenInjectedInto<SqlServerDependencies>();
            Bind<IEntityRecordsExist>().To<SqlServerEntityRecordsExist>().WhenInjectedInto<SqlServerDependencies>();
            Bind<ITflWriter>().To<SqlServerTflWriter>().WhenInjectedInto<SqlServerDependencies>();
            Bind<IViewWriter>().To<SqlServerViewWriter>().WhenInjectedInto<SqlServerDependencies>();
            Bind<ITableQueryWriter>().To<SqlServerTableQueryWriter>().WhenInjectedInto<SqlServerDependencies>();
            Bind<IEntityDropper>().To<SqlServerEntityDropper>().WhenInjectedInto<SqlServerDependencies>();
            Bind<IEntityExists>().To<SqlServerEntityExists>().WhenInjectedInto<SqlServerEntityDropper>();

            // Analysis Services
            Bind<AbstractConnectionDependencies>().To<AnalysisServicesDependencies>().WhenInjectedInto<AnalysisServicesConnection>();
            Bind<AbstractProvider>().To<AnalysisServicesProvider>().WhenInjectedInto<AnalysisServicesDependencies>();
            Bind<IConnectionChecker>().To<AnalysisServicesConnectionChecker>().WhenInjectedInto<AnalysisServicesDependencies>();
            Bind<IScriptRunner>().To<AnalysisServicesScriptRunner>().WhenInjectedInto<AnalysisServicesDependencies>();
            Bind<IProviderSupportsModifier>().To<FalseProviderSupportsModifier>().WhenInjectedInto<AnalysisServicesDependencies>();
            Bind<IEntityRecordsExist>().To<FalseEntityRecordsExist>().WhenInjectedInto<AnalysisServicesDependencies>();
            Bind<ITflWriter>().To<FalseTflWriter>().WhenInjectedInto<AnalysisServicesDependencies>();
            Bind<IViewWriter>().To<FalseViewWriter>().WhenInjectedInto<AnalysisServicesDependencies>();
            Bind<ITableQueryWriter>().To<FalseTableQueryWriter>().WhenInjectedInto<AnalysisServicesDependencies>();
            Bind<IEntityDropper>().To<FalseEntityDropper>().WhenInjectedInto<AnalysisServicesDependencies>();
            Bind<IEntityExists>().To<FalseEntityExists>().WhenInjectedInto<AnalysisServicesEntityDropper>();

            // File (including Excel)
            Bind<AbstractConnectionDependencies>().To<FileDependencies>().WhenInjectedInto<FileConnection>();
            Bind<AbstractProvider>().To<FileProvider>().WhenInjectedInto<FileDependencies>();
            Bind<IConnectionChecker>().To<FileConnectionChecker>().WhenInjectedInto<FileDependencies>();
            Bind<IScriptRunner>().To<FalseScriptRunner>().WhenInjectedInto<FileDependencies>();
            Bind<IProviderSupportsModifier>().To<FalseProviderSupportsModifier>().WhenInjectedInto<FileDependencies>();
            Bind<IEntityRecordsExist>().To<FileEntityRecordsExist>().WhenInjectedInto<FileDependencies>();
            Bind<ITflWriter>().To<FalseTflWriter>().WhenInjectedInto<FileDependencies>();
            Bind<IViewWriter>().To<FalseViewWriter>().WhenInjectedInto<FileDependencies>();
            Bind<ITableQueryWriter>().To<FalseTableQueryWriter>().WhenInjectedInto<FileDependencies>();
            Bind<IEntityDropper>().To<FileEntityDropper>().WhenInjectedInto<FileDependencies>();
            Bind<IEntityExists>().To<FileEntityExists>().WhenInjectedInto<FileEntityDropper>();

            // Folder
            Bind<AbstractConnectionDependencies>().To<FolderDependencies>().WhenInjectedInto<FolderConnection>();
            Bind<AbstractProvider>().To<FolderProvider>().WhenInjectedInto<FolderDependencies>();
            Bind<IConnectionChecker>().To<FolderConnectionChecker>().WhenInjectedInto<FolderDependencies>();
            Bind<IScriptRunner>().To<FalseScriptRunner>().WhenInjectedInto<FolderDependencies>();
            Bind<IProviderSupportsModifier>().To<FalseProviderSupportsModifier>().WhenInjectedInto<FolderDependencies>();
            Bind<IEntityRecordsExist>().To<FolderEntityRecordsExist>().WhenInjectedInto<FolderDependencies>();
            Bind<ITflWriter>().To<FalseTflWriter>().WhenInjectedInto<FolderDependencies>();
            Bind<IViewWriter>().To<FalseViewWriter>().WhenInjectedInto<FolderDependencies>();
            Bind<ITableQueryWriter>().To<FalseTableQueryWriter>().WhenInjectedInto<FolderDependencies>();
            Bind<IEntityDropper>().To<FolderEntityDropper>().WhenInjectedInto<FolderDependencies>();
            Bind<IEntityExists>().To<FolderEntityExists>().WhenInjectedInto<FolderEntityDropper>();

            // Internal Operation
            Bind<AbstractConnectionDependencies>().To<InternalDependencies>().WhenInjectedInto<InternalConnection>();
            Bind<AbstractProvider>().To<InternalProvider>().WhenInjectedInto<InternalDependencies>();
            Bind<IConnectionChecker>().To<InternalConnectionChecker>().WhenInjectedInto<InternalDependencies>();
            Bind<IScriptRunner>().To<FalseScriptRunner>().WhenInjectedInto<InternalDependencies>();
            Bind<IProviderSupportsModifier>().To<FalseProviderSupportsModifier>().WhenInjectedInto<InternalDependencies>();
            Bind<IEntityRecordsExist>().To<FalseEntityRecordsExist>().WhenInjectedInto<InternalDependencies>();
            Bind<ITflWriter>().To<FalseTflWriter>().WhenInjectedInto<InternalDependencies>();
            Bind<IViewWriter>().To<FalseViewWriter>().WhenInjectedInto<InternalDependencies>();
            Bind<ITableQueryWriter>().To<FalseTableQueryWriter>().WhenInjectedInto<InternalDependencies>();
            Bind<IEntityDropper>().To<FalseEntityDropper>().WhenInjectedInto<InternalDependencies>();
            Bind<IEntityExists>().To<FalseEntityExists>().WhenInjectedInto<FalseEntityDropper>();

        }
    }
}
