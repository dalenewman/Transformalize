using Transformalize.Libs.Lucene.Net.Index;

namespace Transformalize.Main.Providers.Lucene {
    public class LuceneIndexReaderFactory {
        public static IndexReader Create(AbstractConnection connection, Entity entity, bool readOnly) {
            return IndexReader.Open(LuceneIndexDirectoryFactory.Create(connection, entity), readOnly);
        }
    }
}