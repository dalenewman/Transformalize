using System.Collections.Generic;

namespace Transformalize.Main.Providers.SqlServer {
    public class SqlServerDependencies : AbstractConnectionDependencies {
        public SqlServerDependencies()
            : base(
                new SqlServerTableQueryWriter(),
                new DefaultConnectionChecker(),
                new SqlServerEntityRecordsExist(),
                new SqlServerEntityDropper(),
                new SqlServerEntityCreator(),
                new List<IViewWriter> { new SqlServerStarViewWriter(), new SqlServerViewWriter() },
                new SqlServerTflWriter(),
                new DatabaseScriptRunner(),
                new SqlServerDataTypeService()
            ) { }
    }
}