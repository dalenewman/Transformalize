using System.IO;
using Transformalize.Main.Providers;

namespace Transformalize.Main
{
    public class FileEntityRecordsExist : IEntityRecordsExist {
        public bool RecordsExist(AbstractConnection connection, string schema, string name)
        {
            return new FileInfo(connection.File).Length > 0;
        }
    }
}