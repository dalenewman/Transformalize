namespace Transformalize.Main.Providers.Log {
    public class MailDependencies : AbstractConnectionDependencies {
        public MailDependencies()
            : base(
                new MailProvider(),
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