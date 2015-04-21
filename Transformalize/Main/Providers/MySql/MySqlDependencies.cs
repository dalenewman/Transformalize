using System.Collections.Generic;

namespace Transformalize.Main.Providers.MySql {
    public class MySqlDependencies : AbstractConnectionDependencies {
        public MySqlDependencies()
            : base(
                new MySqlTableQueryWriter(),
                new DefaultConnectionChecker(),
                new MySqlEntityRecordsExist(),
                new MySqlEntityDropper(),
                new DatabaseEntityCreator(),
                new List<IViewWriter> { new MySqlViewWriter()},
                new MySqlTflWriter(),
                new DatabaseScriptRunner(),
            new MySqlDataTypeService()
            ) { }
    }
}