namespace Transformalize.Main.Providers.AnalysisServices {
    public class AnalysisServicesDependencies : AbstractConnectionDependencies {
        public AnalysisServicesDependencies()
            : base(
                new FalseTableQueryWriter(),
                new AnalysisServicesConnectionChecker(),
                new FalseEntityRecordsExist(),
                new FalseEntityDropper(),
                new FalseEntityCreator(), 
                new FalseViewWriter(),
                new FalseTflWriter(),
                new AnalysisServicesScriptRunner(),
                new FalseDataTypeService()) { }
    }
}