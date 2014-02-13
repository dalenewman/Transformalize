namespace Transformalize.Main.Providers.SqlServer {
    public class SqlServerDependencies : AbstractConnectionDependencies {
        public SqlServerDependencies()
            : base(
                new SqlServerProvider(), 
                new SqlServerTableQueryWriter(), 
                new DefaultConnectionChecker(), 
                new SqlServerEntityRecordsExist(new SqlServerEntityExists()), 
                new SqlServerEntityDropper(new SqlServerEntityExists()), 
                new SqlServerViewWriter(), 
                new SqlServerTflWriter(), 
                new DefaultScriptRunner(),
                new SqlServerProviderSupportsModifier()
            ) { }
    }
}