
using Transformalize.Main.Providers.Lucene;

namespace Transformalize.Main.Providers.ElasticSearch {

    public class LuceneDependencies : AbstractConnectionDependencies {
        public LuceneDependencies() : base(
            new FalseTableQueryWriter(), 
            new LuceneConnectionChecker(),
            new LuceneEntityRecordsExist(),
            new LuceneEntityDropper(),
            new LuceneEntityCreator(),
            new FalseViewWriter(), 
            null, //tflWriter,
            new FalseScriptRunner(), 
            new FalseDataTypeService()) { }
    }

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
                new FalseScriptRunner(),
                new FalseDataTypeService()) { }
    }
}