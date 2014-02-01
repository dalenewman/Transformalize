using System;
using System.IO;

namespace Transformalize.Main.Providers.Folder
{
    public class FolderConnectionChecker : IConnectionChecker
    {
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;

        public bool Check(AbstractConnection connection) {
            return connection.Name.Equals("output", IC) || new DirectoryInfo(connection.Folder).Exists;
        }
        
    }
}