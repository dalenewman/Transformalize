using System.Collections.Generic;

namespace Transformalize.Main.Providers
{
    public class EmptyTableQueryWriter : ITableQueryWriter {
        public string Write(string name, IEnumerable<string> defs, IEnumerable<string> primaryKey, string schema = "dbo", bool ignoreDups = false)
        {
            return string.Empty;
        }

        public string WriteTemporary(string name, Field[] fields, AbstractProvider provider, bool useAlias = true)
        {
            return string.Empty;
        }
    }
}