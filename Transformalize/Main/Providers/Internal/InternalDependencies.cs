using System.Collections.Generic;
using Transformalize.Main.Providers.AnalysisServices;

namespace Transformalize.Main.Providers.Internal {
    public class InternalDependencies : AbstractConnectionDependencies {
        public InternalDependencies()
            : base(
                new FalseTableQueryWriter(),
                new InternalConnectionChecker(),
                new FalseEntityRecordsExist(),
                new FalseEntityDropper(),
                new FalseEntityCreator(),
                new List<IViewWriter> { new FalseViewWriter() },
                new FalseTflWriter(),
                new FalseScriptRunner(),
            new FalseDataTypeService()) { }
    }
}