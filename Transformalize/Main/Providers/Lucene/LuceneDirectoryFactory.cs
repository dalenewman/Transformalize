using System.IO;
using Transformalize.Libs.Lucene.Net.Store;

namespace Transformalize.Main.Providers.Lucene {
    public class LuceneDirectoryFactory {

        public static FSDirectory Create(AbstractConnection connection) {
            var directoryInfo = new DirectoryInfo(connection.Folder);
            if (!directoryInfo.Exists) {
                directoryInfo.Create();
            }
            return FSDirectory.Open(directoryInfo);
        }

        public static string Path(AbstractConnection connection, Entity entity) {
            return connection.Folder.TrimEnd(new[] { '\\' }) + "\\" + entity.OutputName().TrimStart(new[] { '\\' });
        }

        public static FSDirectory Create(AbstractConnection connection, Entity entity) {
            var directoryInfo = new DirectoryInfo(Path(connection, entity));
            if (!directoryInfo.Exists) {
                directoryInfo.Create();
            }
            return FSDirectory.Open(directoryInfo);
        }
    }
}
