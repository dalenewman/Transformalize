using System;
using System.IO;

namespace Transformalize.Main.Providers.Folder
{
    public class FolderEntityRecordsExist : IEntityRecordsExist {
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;

        public bool RecordsExist(AbstractConnection connection, Entity entity) {
            return connection.Name.Equals("output", IC) || new DirectoryInfo(connection.Folder).GetFiles("*.*",SearchOption.TopDirectoryOnly).Length > 0;
        }
    }
}