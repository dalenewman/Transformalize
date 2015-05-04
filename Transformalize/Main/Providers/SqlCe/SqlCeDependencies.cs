using System.Collections.Generic;
using Transformalize.Logging;
using Transformalize.Main.Providers.SqlServer;

namespace Transformalize.Main.Providers.SqlCe {
    public class SqlCeDependencies : AbstractConnectionDependencies {
        public SqlCeDependencies(ILogger logger)
            : base(
                new SqlCeTableQueryWriter(),
                new SqlCeConnectionChecker(logger),
                new SqlCeEntityRecordsExist(),
                new SqlCeEntityDropper(),
                new SqlCeEntityCreator(),
                new List<IViewWriter> { new NullViewWriter() },
                new SqlCeTflWriter(),
                new DatabaseScriptRunner(),
                new SqlServerDataTypeService(), logger
            ) { }
    }
}