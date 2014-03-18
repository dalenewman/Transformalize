using Transformalize.Main.Providers.SqlServer;

namespace Transformalize.Main.Providers.PostgreSql
{
    public class PostgreSqlDependencies : AbstractConnectionDependencies {
        public PostgreSqlDependencies()
            : base(
                new PostgreSqlProvider(),
                new PostgreSqlTableQueryWriter(),
                new DefaultConnectionChecker(),
                new PostgreSqlEntityRecordsExist(),
                new PostgreSqlEntityDropper(),
                new DatabaseEntityCreator(),
                new PostgreSqlViewWriter(),
                new PostgreSqlTflWriter(),
                new DefaultScriptRunner(),
                new FalseProviderSupportsModifier()) { }
    }
}