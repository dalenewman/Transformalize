namespace Transformalize.Main.Providers
{
    public abstract class AbstractConnectionDependencies {

        public AbstractProvider Provider { get; private set; }
        public ITableQueryWriter TableQueryWriter { get; private set; }
        public IConnectionChecker ConnectionChecker { get; private set; }
        public IEntityRecordsExist EntityRecordsExist { get; private set; }
        public IEntityDropper EntityDropper { get; private set; }
        public IViewWriter ViewWriter { get; private set; }
        public ITflWriter TflWriter { get; private set; }
        public IScriptRunner ScriptRunner { get; private set; }
        public IProviderSupportsModifier ProviderSupportsModifier { get; private set; }

        protected AbstractConnectionDependencies(
            AbstractProvider provider,
            ITableQueryWriter tableQueryWriter,
            IConnectionChecker connectionChecker,
            IEntityRecordsExist entityRecordsExist,
            IEntityDropper entityDropper,
            IViewWriter viewWriter,
            ITflWriter tflWriter,
            IScriptRunner scriptRunner,
            IProviderSupportsModifier providerSupportsModifier
            ) {
            Provider = provider;
            TableQueryWriter = tableQueryWriter;
            ConnectionChecker = connectionChecker;
            EntityRecordsExist = entityRecordsExist;
            EntityDropper = entityDropper;
            ViewWriter = viewWriter;
            TflWriter = tflWriter;
            ScriptRunner = scriptRunner;
            ProviderSupportsModifier = providerSupportsModifier;
            }

    }
}