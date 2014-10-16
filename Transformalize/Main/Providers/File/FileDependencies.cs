namespace Transformalize.Main.Providers.File {
    public class FileDependencies : AbstractConnectionDependencies {
        public FileDependencies()
            : base(
                new FalseTableQueryWriter(),
                new FileConnectionChecker(),
                new FileEntityRecordsExist(),
                new FileEntityDropper(),
                new FileEntityCreator(),
                new FalseViewWriter(),
                new FalseTflWriter(),
                new FalseScriptRunner(),
            new FalseDataTypeService()) { }
    }
}