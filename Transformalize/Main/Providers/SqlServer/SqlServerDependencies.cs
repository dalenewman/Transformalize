using System.Collections.Generic;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.SqlServer {
    public class SqlServerDependencies : AbstractConnectionDependencies {
        public SqlServerDependencies(ILogger logger)
            : base(
                new SqlServerTableQueryWriter(),
                new DefaultConnectionChecker(logger),
                new SqlServerEntityRecordsExist(),
                new SqlServerEntityDropper(),
                new SqlServerEntityCreator(logger),
                new List<IViewWriter> { new SqlServerStarViewWriter(), new SqlServerViewWriter() },
                new SqlServerTflWriter(),
                new DatabaseScriptRunner(),
                new SqlServerDataTypeService(),
                logger
            ) { }
    }
}