using System.IO;

namespace Transformalize.Main.Providers.File {
    public class FileEntityExists : IEntityExists {
        public bool Exists(AbstractConnection connection, Entity entity) {
            return new FileInfo(connection.File).Exists;
        }
    }
}