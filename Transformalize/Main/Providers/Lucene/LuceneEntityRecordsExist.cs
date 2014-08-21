using Transformalize.Libs.Lucene.Net.Index;

namespace Transformalize.Main.Providers.Lucene
{
    public class LuceneEntityRecordsExist : IEntityRecordsExist {
        public IEntityExists EntityExists { get; set; }
        public bool RecordsExist(AbstractConnection connection, Entity entity) {
            var checker = new LuceneConnectionChecker();
            if (!checker.Check(connection))
                return false;

            using (var indexDirectory = LuceneIndexDirectoryFactory.Create(connection, entity) ) {
                using (var reader = IndexReader.Open(indexDirectory, true)) {
                    var count = reader.NumDocs();
                    return count > 0;
                }
            }
        }
    }
}