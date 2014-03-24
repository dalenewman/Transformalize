using System.Collections.Generic;

namespace Transformalize.Main.Providers
{
    public class FalseTableQueryWriter : ITableQueryWriter {

        public string CreateTable(string name, IEnumerable<string> defs, string schema = "dbo")
        {
            return string.Empty;
        }

        public string AddPrimaryKey(string name, IEnumerable<string> primaryKey, string schema = "dbo")
        {
            return string.Empty;
        }

        public string DropPrimaryKey(string name, IEnumerable<string> primaryKey, string schema = "dbo")
        {
            return string.Empty;
        }

        public string AddUniqueClusteredIndex(string name, string schema = "dbo")
        {
            return string.Empty;
        }

        public string DropUniqueClusteredIndex(string name, string schema = "dbo")
        {
            return string.Empty;
        }

        public string WriteTemporary(string name, Field[] fields, AbstractConnection connection, bool useAlias = true)
        {
            return string.Empty;
        }
    }
}