using System.Collections.Generic;
using System.Linq;
using Transformalize.Core.Field_;

namespace Transformalize.Providers.SqlServer
{
    public class SqlServerTableQueryWriter : ITableQueryWriter
    {

        private const string CREATE_TABLE_TEMPLATE = @"
            CREATE TABLE [{0}].[{1}](
                {2},
                CONSTRAINT [Pk_{3}_{4}] PRIMARY KEY (
                    {5}
                ) {6}
            );
        ";

        public string Write(string name, IEnumerable<string> defs, IEnumerable<string> primaryKey, string schema = "dbo", bool ignoreDups = false)
        {
            var pk = primaryKey.ToArray();
            var defList = string.Join(",\r\n    ", defs);
            var keyName = string.Join("_", pk).Replace("`", string.Empty).Replace("]", string.Empty).Replace(" ", "_");
            var keyList = string.Join(", ", pk);
            return string.Format(
                CREATE_TABLE_TEMPLATE,
                schema,
                name.Length > 128 ? name.Substring(0, 128) : name,
                defList,
                name.Replace(" ", string.Empty),
                keyName.Length > 128 ? keyName.Substring(0, 128) : keyName,
                keyList,
                ignoreDups ? "WITH (IGNORE_DUP_KEY = ON)" : string.Empty
            );
        }

        public string WriteTemporary(string name, Field[] fields, AbstractProvider provider, bool useAlias = true)
        {
            var defs = useAlias ? new FieldSqlWriter(fields).Alias(provider).DataType().Write() : new FieldSqlWriter(fields).Name(provider).DataType().Write();
            return string.Format(@"DECLARE @{0} AS TABLE({1});", name.TrimStart("@".ToCharArray()), defs);
        }


    }
}
