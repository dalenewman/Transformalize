using System.Collections.Generic;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.Console {
    public class ConsoleDependencies : AbstractConnectionDependencies {
        public ConsoleDependencies(ILogger logger)
            : base(
                new NullTableQueryWriter(),
                new NullConnectionChecker(),
                new NullEntityRecordsExist(),
                new NullEntityDropper(),
                new NullEntityCreator(),
                new List<IViewWriter> { new NullViewWriter() },
                new NullTflWriter(),
                new NullScriptRunner(),
            new NullDataTypeService(), logger) { }
    }
}