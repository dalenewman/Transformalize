using Transformalize.Libs.Lucene.Net.Index;

namespace Transformalize.Main.Providers.Lucene {
    public class LuceneReaderFactory {
        public static IndexReader Create(AbstractConnection connection, Entity entity, bool readOnly) {
            return IndexReader.Open(LuceneDirectoryFactory.Create(connection, entity), readOnly);
        }
    }
}