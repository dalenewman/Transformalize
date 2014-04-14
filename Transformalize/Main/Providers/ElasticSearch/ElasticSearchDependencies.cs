using System;
using System.Security.Cryptography.X509Certificates;

namespace Transformalize.Main.Providers.ElasticSearch {
    public class ElasticSearchDependencies : AbstractConnectionDependencies {
        public ElasticSearchDependencies()
            : base(
                new FalseTableQueryWriter(),
                new ElasticSearchConnectionChecker(),
                new ElasticSearchEntityRecordsExist(),
                new ElasticSearchEntityDropper(),
                new ElasticSearchEntityCreator(),
                new FalseViewWriter(),
                new ElasticSearchTflWriter(),
                new FalseScriptRunner()) { }
    }
}