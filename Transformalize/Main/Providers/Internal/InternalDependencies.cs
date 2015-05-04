using System.Collections.Generic;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.Internal {
    public class InternalDependencies : AbstractConnectionDependencies {
        public InternalDependencies(ILogger logger)
            : base(
                new NullTableQueryWriter(),
                new InternalConnectionChecker(),
                new NullEntityRecordsExist(),
                new NullEntityDropper(),
                new NullEntityCreator(),
                new List<IViewWriter> { new NullViewWriter() },
                new NullTflWriter(),
                new NullScriptRunner(),
            new NullDataTypeService(), logger) { }
    }
}