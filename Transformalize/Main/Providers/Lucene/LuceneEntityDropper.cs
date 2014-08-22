using System.IO;

namespace Transformalize.Main.Providers.Lucene
{
    public class LuceneEntityDropper : IEntityDropper {
        public IEntityExists EntityExists { get; set; }

        public LuceneEntityDropper() {
            EntityExists = new LuceneEntityExists();
        }

        public void Drop(AbstractConnection connection, Entity entity) {
            if (!EntityExists.Exists(connection, entity))
                return;
            new DirectoryInfo(LuceneIndexDirectoryFactory.Path(connection, entity)).Delete(true);
        }
    }
}