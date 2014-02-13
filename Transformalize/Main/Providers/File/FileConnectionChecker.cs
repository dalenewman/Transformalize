using System;
using System.IO;
using Transformalize.Libs.NLog;

namespace Transformalize.Main.Providers.File {
    public class FileConnectionChecker : IConnectionChecker {
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly Logger _log = LogManager.GetLogger(string.Empty);

        public bool Check(AbstractConnection connection) {
            var result = connection.Name.Equals("output", IC) || new FileInfo(connection.File).Exists;
            if (!result) {
                _log.Warn("Connection isn't ready. File {0} doesn't exist!", connection.File);
            }
            return result;
        }
    }
}