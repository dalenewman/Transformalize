using System.IO;

namespace Transformalize.Main.Providers.Folder
{
    public class FolderEntityExists : IEntityExists {
        public bool Exists(AbstractConnection connection, Entity entity) {
            return new DirectoryInfo(connection.File).Exists;
        }
    }
}