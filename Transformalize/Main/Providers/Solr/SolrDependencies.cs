using System.Collections.Generic;

namespace Transformalize.Main.Providers.Solr {
    public class SolrDependencies : AbstractConnectionDependencies {
        public SolrDependencies()
            : base(
                new FalseTableQueryWriter(),
                new SolrConnectionChecker(),
                new SolrEntityRecordsExist(),
                new SolrEntityDropper(),
                new SolrEntityCreator(),
                new List<IViewWriter> { new FalseViewWriter()},
                new SolrTflWriter(),
                new FalseScriptRunner(),
            new FalseDataTypeService()) { }
    }
}