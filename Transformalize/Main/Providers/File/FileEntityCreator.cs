using System.IO;

namespace Transformalize.Main.Providers
{
    public class FileEntityCreator : IEntityCreator {
        public IEntityExists EntityExists { get; set; }

        public FileEntityCreator() {
            EntityExists = new FileEntityExists();
        }

        public void Create(AbstractConnection connection, Process process, Entity entity) {
            System.IO.File.WriteAllText(new FileInfo(connection.File).FullName, string.Empty);
        }
    }
}