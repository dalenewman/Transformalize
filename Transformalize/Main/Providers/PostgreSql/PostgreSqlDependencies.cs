using System.Collections.Generic;
using Transformalize.Main.Providers.SqlServer;

namespace Transformalize.Main.Providers.PostgreSql {
    public class PostgreSqlDependencies : AbstractConnectionDependencies {
        public PostgreSqlDependencies()
            : base(
                new PostgreSqlTableQueryWriter(),
                new DefaultConnectionChecker(),
                new PostgreSqlEntityRecordsExist(),
                new PostgreSqlEntityDropper(),
                new DatabaseEntityCreator(),
                new List<IViewWriter> { new PostgreSqlViewWriter()},
                new PostgreSqlTflWriter(),
                new DatabaseScriptRunner(),
            new PostgreSqlDataTypeService()) { }
    }
}