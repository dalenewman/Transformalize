using System.Collections.Generic;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.AnalysisServices {
    public class AnalysisServicesDependencies : AbstractConnectionDependencies {
        public AnalysisServicesDependencies(ILogger logger)
            : base(
                new NullTableQueryWriter(),
                new AnalysisServicesConnectionChecker(logger),
                new NullEntityRecordsExist(),
                new NullEntityDropper(),
                new NullEntityCreator(),
                new List<IViewWriter> { new NullViewWriter() },
                new NullTflWriter(),
                new AnalysisServicesScriptRunner(),
                new NullDataTypeService(), logger) { }
    }
}