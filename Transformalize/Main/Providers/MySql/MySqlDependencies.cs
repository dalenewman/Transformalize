using System.Collections.Generic;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.MySql {
    public class MySqlDependencies : AbstractConnectionDependencies {
        public MySqlDependencies(ILogger logger)
            : base(
                new MySqlTableQueryWriter(),
                new DefaultConnectionChecker(logger),
                new MySqlEntityRecordsExist(),
                new MySqlEntityDropper(),
                new DatabaseEntityCreator(),
                new List<IViewWriter> { new MySqlViewWriter() },
                new MySqlTflWriter(),
                new DatabaseScriptRunner(),
                new MySqlDataTypeService(),
                logger
            ) { }
    }
}