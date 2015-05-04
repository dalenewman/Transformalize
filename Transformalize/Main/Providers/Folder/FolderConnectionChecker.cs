using System;
using System.IO;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.Folder {
    public class FolderConnectionChecker : IConnectionChecker {
        private readonly ILogger _logger;
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;

        public FolderConnectionChecker(ILogger logger) {
            _logger = logger;
        }

        public bool Check(AbstractConnection connection) {
            var result = connection.Name.Equals("output", IC) || new DirectoryInfo(connection.Folder).Exists;
            if (!result) {
                _logger.Warn("Could not verify that '{0}' exists.", connection.Folder);
            }
            return result;
        }

    }
}