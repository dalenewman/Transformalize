namespace Transformalize.Main.Providers.Internal
{
    public class MySqlDependencies : AbstractConnectionDependencies
    {
        public MySqlDependencies(AbstractProvider provider, ITableQueryWriter tableQueryWriter, IConnectionChecker connectionChecker, IEntityRecordsExist entityRecordsExist, IEntityDropper entityDropper, IViewWriter viewWriter, ITflWriter tflWriter, IScriptRunner scriptRunner, IProviderSupportsModifier providerSupportsModifier) : base(provider, tableQueryWriter, connectionChecker, entityRecordsExist, entityDropper, viewWriter, tflWriter, scriptRunner, providerSupportsModifier) {}
    }
}