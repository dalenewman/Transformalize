using System.IO;

namespace Transformalize.Main.Providers {

    public class FileEntityDropper : IEntityDropper {
        public IEntityExists EntityExists { get; set; }

        public FileEntityDropper() {
            EntityExists = new FileEntityExists();
        }

        public void Drop(AbstractConnection connection, Entity entity) {
            new FileInfo(connection.File).Delete();
        }
    }
}