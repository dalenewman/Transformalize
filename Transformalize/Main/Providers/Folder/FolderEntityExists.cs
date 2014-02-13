using System.IO;
using Transformalize.Main.Providers;

namespace Transformalize.Main
{
    public class FolderEntityExists : IEntityExists {
        public bool Exists(AbstractConnection connection, Entity entity) {
            return new DirectoryInfo(connection.File).Exists;
        }
    }
}