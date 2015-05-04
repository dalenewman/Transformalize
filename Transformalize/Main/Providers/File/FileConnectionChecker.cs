using System.IO;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.File {
    public class FileConnectionChecker : IConnectionChecker {
        private readonly ILogger _logger;

        public FileConnectionChecker(ILogger logger) {
            _logger = logger;
        }

        public bool Check(AbstractConnection connection) {
            var fileInfo = new FileInfo(connection.File);
            if (!fileInfo.Exists) {
                _logger.Warn("File {0} does not exist.  Creating it.", fileInfo.Name);
                System.IO.File.Create(fileInfo.FullName).Dispose();
            }
            return true;
        }
    }
}