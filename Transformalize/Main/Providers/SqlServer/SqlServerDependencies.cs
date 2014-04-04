namespace Transformalize.Main.Providers.SqlServer {
    public class SqlServerDependencies : AbstractConnectionDependencies {
        public SqlServerDependencies()
            : base(
                new SqlServerTableQueryWriter(), 
                new DefaultConnectionChecker(), 
                new SqlServerEntityRecordsExist(), 
                new SqlServerEntityDropper(),
                new SqlServerEntityCreator(),
                new SqlServerViewWriter(), 
                new SqlServerTflWriter(), 
                new DatabaseScriptRunner()
            ) { }
    }
}