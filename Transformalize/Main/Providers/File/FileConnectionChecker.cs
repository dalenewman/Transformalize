using System;
using System.IO;

namespace Transformalize.Main.Providers.File {
    public class FileConnectionChecker : IConnectionChecker {
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;

        public bool Check(AbstractConnection connection) {
            return connection.Name.Equals("output", IC) || new FileInfo(connection.File).Exists;
        }
    }
}