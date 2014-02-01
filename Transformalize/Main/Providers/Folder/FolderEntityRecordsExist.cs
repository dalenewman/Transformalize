using System;
using System.IO;
using Transformalize.Main.Providers;

namespace Transformalize.Main
{
    public class FolderEntityRecordsExist : IEntityRecordsExist {
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;

        public bool RecordsExist(AbstractConnection connection, string schema, string name) {
            return connection.Name.Equals("output", IC) || new DirectoryInfo(connection.Folder).GetFiles("*.*",SearchOption.TopDirectoryOnly).Length > 0;
        }
    }
}