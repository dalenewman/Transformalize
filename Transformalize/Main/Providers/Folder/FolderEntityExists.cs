using System.IO;
using Transformalize.Main.Providers;

namespace Transformalize.Main
{
    public class FolderEntityExists : IEntityExists {
        public bool Exists(AbstractConnection connection, string schema, string name) {
            return new DirectoryInfo(connection.File).Exists;
        }
    }
}