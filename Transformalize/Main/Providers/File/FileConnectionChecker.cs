using System.IO;
using Transformalize.Libs.NLog;

namespace Transformalize.Main.Providers.File {
    public class FileConnectionChecker : IConnectionChecker {
        private readonly Logger _log = LogManager.GetLogger("tfl");

        public bool Check(AbstractConnection connection) {
            var fileInfo = new FileInfo(connection.File);
            if (!fileInfo.Exists) {
                _log.Warn("Creating empty file {0}", fileInfo.Name);
                System.IO.File.Create(fileInfo.FullName).Dispose();
            }
            return true;
        }
    }
}