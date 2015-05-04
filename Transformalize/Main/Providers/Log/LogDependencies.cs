using System.Collections.Generic;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.Log
{
    public class LogDependencies : AbstractConnectionDependencies {
        public LogDependencies(ILogger logger)
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