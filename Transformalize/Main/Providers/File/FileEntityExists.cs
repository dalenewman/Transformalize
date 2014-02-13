using System.IO;
using Transformalize.Main.Providers;

namespace Transformalize.Main {
    public class FileEntityExists : IEntityExists {
        public bool Exists(AbstractConnection connection, Entity entity) {
            return new FileInfo(connection.File).Exists;
        }
    }
}