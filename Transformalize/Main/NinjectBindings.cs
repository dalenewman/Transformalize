using System;
using System.Data;
using Transformalize.Main.Providers;
using Transformalize.Main.Providers.AnalysisServices;
using Transformalize.Main.Providers.Console;
using Transformalize.Main.Providers.File;
using Transformalize.Main.Providers.Folder;
using Transformalize.Main.Providers.Internal;
using Transformalize.Main.Providers.Log;
using Transformalize.Main.Providers.MySql;
using Transformalize.Main.Providers.PostgreSql;
using Transformalize.Main.Providers.SqlCe4;
using Transformalize.Main.Providers.SqlServer;

namespace Transformalize.Main {

    public class NinjectBindings : Libs.Ninject.Modules.NinjectModule {

        public override void Load() {
            // databases
            Bind<AbstractConnectionDependencies>().To<SqlServerDependencies>().WhenInjectedInto<SqlServerConnection>();
            Bind<AbstractConnectionDependencies>().To<MySqlDependencies>().WhenInjectedInto<MySqlConnection>();
            Bind<AbstractConnectionDependencies>().To<PostgreSqlDependencies>().WhenInjectedInto<PostgreSqlConnection>();
            Bind<AbstractConnectionDependencies>().To<SqlCe4Dependencies>().WhenInjectedInto<SqlCe4Connection>();

            // others
            Bind<AbstractConnectionDependencies>().To<AnalysisServicesDependencies>().WhenInjectedInto<AnalysisServicesConnection>();
            Bind<AbstractConnectionDependencies>().To<FileDependencies>().WhenInjectedInto<FileConnection>();
            Bind<AbstractConnectionDependencies>().To<FolderDependencies>().WhenInjectedInto<FolderConnection>();
            Bind<AbstractConnectionDependencies>().To<InternalDependencies>().WhenInjectedInto<InternalConnection>();
            Bind<AbstractConnectionDependencies>().To<ConsoleDependencies>().WhenInjectedInto<ConsoleConnection>();
            Bind<AbstractConnectionDependencies>().To<LogDependencies>().WhenInjectedInto<LogConnection>();
            Bind<AbstractConnectionDependencies>().To<MailDependencies>().WhenInjectedInto<MailConnection>();
            Bind<AbstractConnectionDependencies>().To<HtmlDependencies>().WhenInjectedInto<HtmlConnection>();
        }
    }
}
