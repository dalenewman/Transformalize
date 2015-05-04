using System.Collections.Generic;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.File {
    public class FileDependencies : AbstractConnectionDependencies {
        public FileDependencies(ILogger logger)
            : base(
                new NullTableQueryWriter(),
                new FileConnectionChecker(logger),
                new FileEntityRecordsExist(),
                new FileEntityDropper(),
                new FileEntityCreator(),
                new List<IViewWriter> { new NullViewWriter() },
                new NullTflWriter(),
                new NullScriptRunner(),
            new NullDataTypeService(), logger) { }
    }
}