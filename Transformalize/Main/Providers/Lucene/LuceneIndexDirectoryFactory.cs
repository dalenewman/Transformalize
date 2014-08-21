using System.IO;
using Transformalize.Libs.Lucene.Net.Store;

namespace Transformalize.Main.Providers.Lucene {
    public class LuceneIndexDirectoryFactory {

        public static FSDirectory Create(AbstractConnection connection) {
            var directoryInfo = new DirectoryInfo(connection.Folder);
            if (!directoryInfo.Exists) {
                directoryInfo.Create();
            }
            return FSDirectory.Open(directoryInfo);
        }

        public static FSDirectory Create(AbstractConnection connection, Entity entity) {
            var path = connection.Folder.TrimEnd(new[] { '\\' }) + "\\" + entity.OutputName().TrimStart(new[] { '\\' });
            var directoryInfo = new DirectoryInfo(path);
            if (!directoryInfo.Exists) {
                directoryInfo.Create();
            }
            return FSDirectory.Open(directoryInfo);
        }
    }
}
