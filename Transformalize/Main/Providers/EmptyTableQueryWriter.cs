using System.Collections.Generic;

namespace Transformalize.Main.Providers
{
    public class EmptyTableQueryWriter : ITableQueryWriter {
        public string CreateTable(string name, IEnumerable<string> defs, string schema)
        {
            return string.Empty;
        }

        public string AddPrimaryKey(string name, string schema, IEnumerable<string> primaryKey)
        {
            return string.Empty;
        }

        public string DropPrimaryKey(string name, string schema, IEnumerable<string> primaryKey)
        {
            return string.Empty;
        }

        public string AddUniqueClusteredIndex(string name, string schema)
        {
            return string.Empty;
        }

        public string DropUniqueClusteredIndex(string name, string schema)
        {
            return string.Empty;
        }

        public string WriteTemporary(string name, Field[] fields, AbstractProvider provider, bool useAlias = true)
        {
            return string.Empty;
        }
    }
}