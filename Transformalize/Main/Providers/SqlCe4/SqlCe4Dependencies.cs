namespace Transformalize.Main.Providers.SqlCe4 {
    public class SqlCe4Dependencies : AbstractConnectionDependencies {
        public SqlCe4Dependencies()
            : base(
                new SqlCe4TableQueryWriter(),
                new SqlCe4ConnectionChecker(),
                new SqlCe4EntityRecordsExist(),
                new SqlCe4EntityDropper(),
                new SqlCe4EntityCreator(),
                new FalseViewWriter(),
                new SqlCe4TflWriter(),
                new DefaultScriptRunner()
            ) { }
    }
}