namespace Transformalize.Main.Providers.Lucene {
    public class LuceneEntityCreator : IEntityCreator {

        public IEntityExists EntityExists { get; set; }

        public LuceneEntityCreator() {
            EntityExists = new LuceneEntityExists();
        }

        public void Create(AbstractConnection connection, Process process, Entity entity) {
            LuceneDirectoryFactory.Create(connection, entity);
        }

    }
}