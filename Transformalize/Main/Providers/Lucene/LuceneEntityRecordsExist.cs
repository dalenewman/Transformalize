using System.IO;
using Transformalize.Libs.Lucene.Net.Index;

namespace Transformalize.Main.Providers.Lucene {
    public class LuceneEntityRecordsExist : IEntityRecordsExist {
        public IEntityExists EntityExists { get; set; }

        public LuceneEntityRecordsExist() {
            EntityExists = new LuceneEntityExists();

        }

        public bool RecordsExist(AbstractConnection connection, Entity entity) {

            var checker = new LuceneConnectionChecker(entity.ProcessName, connection.Logger);
            if (!checker.Check(connection))
                return false;

            var directoryInfo = new DirectoryInfo(LuceneDirectoryFactory.Path(connection, entity));

            if (!directoryInfo.Exists)
                return false;

            if(directoryInfo.GetFiles().Length == 0)
                return false;

            using (var indexDirectory = LuceneDirectoryFactory.Create(connection, entity)) {
                using (var reader = IndexReader.Open(indexDirectory, true)) {
                    var count = reader.NumDocs();
                    return count > 0;
                }
            }
        }
    }
}