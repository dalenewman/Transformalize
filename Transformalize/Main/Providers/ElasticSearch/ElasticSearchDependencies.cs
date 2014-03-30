using System;

namespace Transformalize.Main.Providers.ElasticSearch {
    public class ElasticSearchDependencies : AbstractConnectionDependencies {
        public ElasticSearchDependencies()
            : base(
                new FalseTableQueryWriter(),
                new ElasticSearchConnectionChecker(),
                new ElasticSearchEntityRecordsExist(),
                new ElasticSearchEntityDropper(),
                new FalseEntityCreator(),
                new FalseViewWriter(),
                new FalseTflWriter(),
                new FalseScriptRunner()) { }
    }
}