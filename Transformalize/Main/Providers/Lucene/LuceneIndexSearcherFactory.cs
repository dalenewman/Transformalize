using Transformalize.Libs.Lucene.Net.Search;

namespace Transformalize.Main.Providers.Lucene {
    public class LuceneIndexSearcherFactory {
        public static IndexSearcher Create(AbstractConnection connection, Entity entity) {
            using (var reader = LuceneIndexReaderFactory.Create(connection, entity, true)) {
                return new IndexSearcher(reader);
            }
        }
    }
}