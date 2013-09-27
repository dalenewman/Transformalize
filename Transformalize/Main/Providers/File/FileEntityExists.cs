using System.IO;
using Transformalize.Main.Providers;

namespace Transformalize.Main
{
    public class FileEntityExists : IEntityExists
    {
        public bool Exists(AbstractConnection connection, string schema, string name)
        {
            return new FileInfo(connection.File).Exists;
        }
    }
}