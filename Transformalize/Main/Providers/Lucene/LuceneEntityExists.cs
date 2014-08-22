using System.IO;

namespace Transformalize.Main.Providers.Lucene {
    public class LuceneEntityExists : IEntityExists {
        public bool Exists(AbstractConnection connection, Entity entity) {
            if (!new DirectoryInfo(LuceneIndexDirectoryFactory.Path(connection, entity)).Exists)
                return false;
            using (var dir = LuceneIndexDirectoryFactory.Create(connection, entity)) {
                return dir.ListAll().Length > 0;
            }
        }
    }
}