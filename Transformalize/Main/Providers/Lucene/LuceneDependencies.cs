namespace Transformalize.Main.Providers.Lucene {
    public class LuceneDependencies : AbstractConnectionDependencies {
        public LuceneDependencies()
            : base(
                new FalseTableQueryWriter(),
                new LuceneConnectionChecker(),
                new LuceneEntityRecordsExist(),
                new LuceneEntityDropper(),
                new LuceneEntityCreator(),
                new FalseViewWriter(),
                new LuceneTflWriter(),
                new FalseScriptRunner(),
                new FalseDataTypeService()) { }
    }
}