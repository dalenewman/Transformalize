namespace Transformalize.Main.Providers.Lucene {
    public class LuceneEntityExists : IEntityExists {
        public bool Exists(AbstractConnection connection, Entity entity) {
            using (var dir = LuceneDirectoryFactory.Create(connection, entity)) {
                return dir.ListAll().Length > 0;
            }
        }
    }
}