using Transformalize.Libs.Lucene.Net.Search;

namespace Transformalize.Main.Providers.Lucene {
    public class LuceneSearcherFactory {
        public static IndexSearcher Create(AbstractConnection connection, Entity entity) {
            return new IndexSearcher(LuceneReaderFactory.Create(connection, entity, true));
        }
    }
}