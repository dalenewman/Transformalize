using System.Collections.Generic;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.Solr {
    public class SolrDependencies : AbstractConnectionDependencies {
        public SolrDependencies(ILogger logger)
            : base(
                new NullTableQueryWriter(),
                new SolrConnectionChecker(logger),
                new SolrEntityRecordsExist(),
                new SolrEntityDropper(),
                new SolrEntityCreator(logger),
                new List<IViewWriter> { new NullViewWriter()},
                new SolrTflWriter(),
                new NullScriptRunner(),
            new NullDataTypeService(), logger) { }
    }
}