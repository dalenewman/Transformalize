using System.Collections.Generic;

namespace Transformalize.Main.Providers {
    public class NullTableQueryWriter : ITableQueryWriter {

        public string CreateTable(string name, IEnumerable<string> defs) {
            return string.Empty;
        }

        public string AddPrimaryKey(string name, IEnumerable<string> primaryKey) {
            return string.Empty;
        }

        public string AddUniqueClusteredIndex(string name) {
            return string.Empty;
        }

        public string WriteTemporary(AbstractConnection connection, string name, Fields fields, bool useAlias = true) {
            return string.Empty;
        }
    }
}