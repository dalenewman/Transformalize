using System.Collections.Generic;
using System.Linq;
using Transformalize.Core.Field_;

namespace Transformalize.Providers.MySql
{
    public class MySqlTableQueryWriter : ITableQueryWriter
    {
        private const string CREATE_TABLE_TEMPLATE = @"
            CREATE TABLE `{0}`(
                {1},
                CONSTRAINT `Pk_{2}_{3}` PRIMARY KEY (
                    {4}
                )
            );
        ";

        public string Write(string name, IEnumerable<string> defs, IEnumerable<string> primaryKey, string schema = "dbo", bool ignoreDups = false)
        {
            var pk = primaryKey.ToArray();
            var defList = string.Join(",\r\n    ", defs);
            var keyName = string.Join("_", pk).Replace("`", string.Empty).Replace(" ", "_");
            var keyList = string.Join(", ", pk);
            return string.Format(
                CREATE_TABLE_TEMPLATE,
                name.Length > 128 ? name.Substring(0, 128) : name,
                defList,
                name.Replace(" ", string.Empty),
                keyName.Length > 128 ? keyName.Substring(0, 128) : keyName,
                keyList
                );
        }

        public string WriteTemporary(string name, Field[] fields, AbstractProvider provider, bool useAlias = true)
        {
            var safeName = provider.Enclose(name.TrimStart("@".ToCharArray()));
            var defs = useAlias ? new FieldSqlWriter(fields).Alias(provider).DataType().Write() : new FieldSqlWriter(fields).Name(provider).DataType().Write();
            return string.Format(@"CREATE TEMPORARY TABLE {0}({1}) ENGINE = MEMORY;", safeName, defs);
        }
    }
}
