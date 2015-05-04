using System.Collections.Generic;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.PostgreSql {
    public class PostgreSqlDependencies : AbstractConnectionDependencies {
        public PostgreSqlDependencies(ILogger logger)
            : base(
                new PostgreSqlTableQueryWriter(),
                new DefaultConnectionChecker(logger),
                new PostgreSqlEntityRecordsExist(),
                new PostgreSqlEntityDropper(),
                new DatabaseEntityCreator(),
                new List<IViewWriter> { new PostgreSqlViewWriter()},
                new PostgreSqlTflWriter(),
                new DatabaseScriptRunner(),
            new PostgreSqlDataTypeService(), logger) { }
    }
}