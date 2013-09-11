using System.Collections.Generic;
using Transformalize.Core.Field_;

namespace Transformalize.Providers
{
    public interface ITableQueryWriter
    {
        string Write(string name, IEnumerable<string> defs, IEnumerable<string> primaryKey, string schema = "dbo", bool ignoreDups = false);
        string WriteTemporary(string name, Field[] fields, AbstractProvider provider, bool useAlias = true);
    }
}