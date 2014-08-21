using Transformalize.Libs.Lucene.Net.Index;

namespace Transformalize.Main.Providers.Lucene {
    public class LuceneIndexReaderFactory {
        public static IndexReader Create(AbstractConnection connection, bool readOnly) {
            using (var dir = LuceneIndexDirectoryFactory.Create(connection)) {
                return IndexReader.Open(dir, readOnly);
            }
        }

        public static IndexReader Create(AbstractConnection connection, Entity entity, bool readOnly) {
            using (var dir = LuceneIndexDirectoryFactory.Create(connection, entity)) {
                return IndexReader.Open(dir, readOnly);
            }
        }
    }
}