namespace Transformalize.Main.Providers.ElasticSearch
{
    public class ElasticSearchDependencies : AbstractConnectionDependencies {
        public ElasticSearchDependencies()
            : base(
                new FalseTableQueryWriter(),
                new FalseConnectionChecker(),
                new FalseEntityRecordsExist(),
                new FalseEntityDropper(),
                new FalseEntityCreator(),
                new FalseViewWriter(),
                new FalseTflWriter(),
                new FalseScriptRunner()) { }
    }
}