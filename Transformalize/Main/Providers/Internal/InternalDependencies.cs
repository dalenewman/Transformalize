namespace Transformalize.Main.Providers.Internal {
    public class InternalDependencies : AbstractConnectionDependencies {
        public InternalDependencies()
            : base(
                new InternalProvider(),
                new FalseTableQueryWriter(),
                new InternalConnectionChecker(),
                new FalseEntityRecordsExist(),
                new FalseEntityDropper(),
                new FalseEntityCreator(),
                new FalseViewWriter(),
                new FalseTflWriter(),
                new FalseScriptRunner(),
                new FalseProviderSupportsModifier()) { }
    }
}