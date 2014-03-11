using System.IO;

namespace Transformalize.Main.Providers.Folder {
    public class FolderEntityDropper : IEntityDropper {
        public IEntityExists EntityExists { get; set; }

        public FolderEntityDropper() {
            EntityExists = new FolderEntityExists();
        }

        public void Drop(AbstractConnection connection, Entity entity) {
            new DirectoryInfo(connection.Folder).Delete(true);
        }
    }
}