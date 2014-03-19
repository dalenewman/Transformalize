using System.Collections.Generic;
using Transformalize.Main.Providers.SqlServer;

namespace Transformalize.Main.Providers.PostgreSql
{
    public class PostgreSqlTableQueryWriter : QueryWriter, ITableQueryWriter {

        public string CreateTable(string name, IEnumerable<string> defs, string schema) {
            var defList = string.Join(",\r\n    ", defs);
            return string.Format(
                "CREATE TABLE \"{0}\"({1});",
                SqlIdentifier(name),
                defList
                );
        }

        public string AddPrimaryKey(string name, IEnumerable<string> primaryKey, string schema) {
            throw new System.NotImplementedException();
        }

        public string DropPrimaryKey(string name, IEnumerable<string> primaryKey, string schema) {
            throw new System.NotImplementedException();
        }

        public string AddUniqueClusteredIndex(string name, string schema) {
            throw new System.NotImplementedException();
        }

        public string DropUniqueClusteredIndex(string name, string schema) {
            throw new System.NotImplementedException();
        }

        public string WriteTemporary(string name, Field[] fields, AbstractProvider provider, bool useAlias = true) {
            var safeName = provider.Enclose(name.TrimStart("@".ToCharArray()));
            var defs = useAlias ? new FieldSqlWriter(fields).Alias(provider).DataType(new PostgreSqlDataTypeService()).Write() : new FieldSqlWriter(fields).Name(provider).DataType(new PostgreSqlDataTypeService()).Write();
            return string.Format(@"CREATE TEMP TABLE {0}({1});", safeName, defs);
        }
    }
}