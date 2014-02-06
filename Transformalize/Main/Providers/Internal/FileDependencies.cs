namespace Transformalize.Main.Providers.Internal
{
    public class FileDependencies : AbstractConnectionDependencies
    {
        public FileDependencies(AbstractProvider provider, ITableQueryWriter tableQueryWriter, IConnectionChecker connectionChecker, IEntityRecordsExist entityRecordsExist, IEntityDropper entityDropper, IViewWriter viewWriter, ITflWriter tflWriter, IScriptRunner scriptRunner, IProviderSupportsModifier providerSupportsModifier) : base(provider, tableQueryWriter, connectionChecker, entityRecordsExist, entityDropper, viewWriter, tflWriter, scriptRunner, providerSupportsModifier) {}
    }
}