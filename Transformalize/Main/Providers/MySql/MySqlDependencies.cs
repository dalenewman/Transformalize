using Transformalize.Main.Providers.SqlServer;

namespace Transformalize.Main.Providers.MySql {
    public class MySqlDependencies : AbstractConnectionDependencies {
        public MySqlDependencies()
            : base(
                new MySqlTableQueryWriter(),
                new DefaultConnectionChecker(),
                new MySqlEntityRecordsExist(),
                new MySqlEntityDropper(),
                new DatabaseEntityCreator(),
                new MySqlViewWriter(),
                new MySqlTflWriter(),
                new DefaultScriptRunner()) { }
    }
}