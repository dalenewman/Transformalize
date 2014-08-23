using Transformalize.Libs.Lucene.Net.Index;
using Transformalize.Libs.Lucene.Net.Search;

namespace Transformalize.Main.Providers.Lucene {
    public class LuceneIndexSearcherFactory {
        public static IndexSearcher Create(AbstractConnection connection, Entity entity) {
            return new IndexSearcher(LuceneIndexReaderFactory.Create(connection, entity, true));
        }
    }
}