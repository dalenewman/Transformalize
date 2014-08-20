using Transformalize.Main.Providers.Lucene;

namespace Transformalize.Main.Providers.ElasticSearch {
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