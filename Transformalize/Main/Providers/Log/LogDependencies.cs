using System.Collections.Generic;
using Transformalize.Main.Providers.AnalysisServices;

namespace Transformalize.Main.Providers.Log
{
    public class LogDependencies : AbstractConnectionDependencies {
        public LogDependencies()
            : base(
                new FalseTableQueryWriter(),
                new FalseConnectionChecker(),
                new FalseEntityRecordsExist(),
                new FalseEntityDropper(),
                new FalseEntityCreator(),
                new List<IViewWriter> { new FalseViewWriter() },
                new FalseTflWriter(),
                new FalseScriptRunner(),
            new FalseDataTypeService()) { }
    }
}