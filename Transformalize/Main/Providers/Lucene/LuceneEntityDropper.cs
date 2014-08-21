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
            using (var dir = LuceneIndexDirectoryFactory.Create(connection, entity)) {
                dir.Directory.Delete(true);
            }
        }
    }
}