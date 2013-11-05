using System.IO;

namespace Transformalize.Main.Providers {

    public class FileEntityDropper : IEntityDropper {
        public void Drop(AbstractConnection connection, string schema, string name) {
            new FileInfo(connection.File).Delete();
        }
    }

    public class InternalEntityDropper : IEntityDropper {
        public void Drop(AbstractConnection connection, string schema, string name) {
            connection.InputOperation = null;
        }
    }
}