
using System.Collections.Generic;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.ElasticSearch {
    public class ElasticSearchDependencies : AbstractConnectionDependencies {
        public ElasticSearchDependencies(ILogger logger)
            : base(
                new NullTableQueryWriter(),
                new ElasticSearchConnectionChecker(logger),
                new ElasticSearchEntityRecordsExist(),
                new ElasticSearchEntityDropper(),
                new ElasticSearchEntityCreator(logger),
                new List<IViewWriter> { new NullViewWriter() },
                new ElasticSearchTflWriter(),
                new NullScriptRunner(),
                new NullDataTypeService(), logger) { }
    }
}