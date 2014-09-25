using System.IO;

namespace Transformalize.Main.Providers.File {
    public class FileConnectionChecker : IConnectionChecker {

        public bool Check(AbstractConnection connection) {
            var fileInfo = new FileInfo(connection.File);
            if (!fileInfo.Exists) {
                TflLogger.Warn(string.Empty, string.Empty, "Creating empty file {0}", fileInfo.Name);
                System.IO.File.Create(fileInfo.FullName).Dispose();
            }
            return true;
        }
    }
}