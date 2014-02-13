using Transformalize.Main.Providers.Internal;

namespace Transformalize.Main.Providers.AnalysisServices {
    public class AnalysisServicesDependencies : AbstractConnectionDependencies {
        public AnalysisServicesDependencies()
            : base(
                new AnalysisServicesProvider(),
                new FalseTableQueryWriter(),
                new AnalysisServicesConnectionChecker(),
                new FalseEntityRecordsExist(),
                new FalseEntityDropper(),
                new FalseViewWriter(),
                new FalseTflWriter(),
                new AnalysisServicesScriptRunner(),
                new FalseProviderSupportsModifier()) { }
    }
}