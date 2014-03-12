namespace Transformalize.Main.Providers.Log
{
    public class LogDependencies : AbstractConnectionDependencies {
        public LogDependencies()
            : base(
                new LogProvider(),
                new FalseTableQueryWriter(),
                new FalseConnectionChecker(),
                new FalseEntityRecordsExist(),
                new FalseEntityDropper(),
                new FalseEntityCreator(),
                new FalseViewWriter(),
                new FalseTflWriter(),
                new FalseScriptRunner(),
                new FalseProviderSupportsModifier()) { }
    }
}