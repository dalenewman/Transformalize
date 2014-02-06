namespace Transformalize.Main.Providers.Internal
{
    public class FolderDependencies : AbstractConnectionDependencies
    {
        public FolderDependencies(AbstractProvider provider, ITableQueryWriter tableQueryWriter, IConnectionChecker connectionChecker, IEntityRecordsExist entityRecordsExist, IEntityDropper entityDropper, IViewWriter viewWriter, ITflWriter tflWriter, IScriptRunner scriptRunner, IProviderSupportsModifier providerSupportsModifier) : base(provider, tableQueryWriter, connectionChecker, entityRecordsExist, entityDropper, viewWriter, tflWriter, scriptRunner, providerSupportsModifier) {}
    }
}