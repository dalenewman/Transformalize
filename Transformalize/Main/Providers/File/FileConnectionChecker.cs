using System.IO;

namespace Transformalize.Main.Providers.File {
    public class FileConnectionChecker : IConnectionChecker {
        public bool Check(AbstractConnection connection) {
            return new FileInfo(connection.File).Exists;
        }
    }
}