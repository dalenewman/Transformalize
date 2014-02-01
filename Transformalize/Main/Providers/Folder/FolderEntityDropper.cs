using System.IO;

namespace Transformalize.Main.Providers
{
    public class FolderEntityDropper : IEntityDropper {
        public IEntityExists EntityExists { get; set; }
        public void Drop(AbstractConnection connection, string schema, string name) {
            new DirectoryInfo(connection.Folder).Delete(true);
        }
    }
}