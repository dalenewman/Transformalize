using System;
using System.IO;

namespace Transformalize.Main.Providers.File {

    public class FileEntityRecordsExist : IEntityRecordsExist {
        public IEntityExists EntityExists { get; set; }
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;

        public bool RecordsExist(AbstractConnection connection, Entity entity) {
            return connection.Name.Equals("output", IC) || new FileInfo(connection.File).Length > 0;
        }
    }
}