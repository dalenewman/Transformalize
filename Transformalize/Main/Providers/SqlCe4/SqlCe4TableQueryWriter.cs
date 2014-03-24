using System.Collections.Generic;
using System.Linq;

namespace Transformalize.Main.Providers.SqlCe4 {

    public class SqlCe4TableQueryWriter : QueryWriter, ITableQueryWriter {

        public string CreateTable(string name, IEnumerable<string> defs, string schema = "dbo") {
            var defList = string.Join(",", defs);
            return string.Format(
                "CREATE TABLE [{0}]({1});",
                SqlIdentifier(name),
                defList
                );
        }

        public string AddPrimaryKey(string name, IEnumerable<string> primaryKey, string schema = "dbo") {
            var pk = primaryKey.ToArray();
            var keyList = string.Join(", ", pk).Replace(" ASC", string.Empty);
            return string.Format(
                "ALTER TABLE [{0}] ADD CONSTRAINT [PK_{0}_{1}] PRIMARY KEY ({2});",
                SqlIdentifier(name),
                KeyName(pk),
                keyList
            );
        }

        public string DropPrimaryKey(string name, IEnumerable<string> primaryKey, string schema = "dbo") {
            var pk = primaryKey.ToArray();
            return string.Format(
                "ALTER TABLE [{0}] DROP CONSTRAINT [PK_{0}_{1}];",
                SqlIdentifier(name),
                KeyName(pk)
                );
        }

        public string AddUniqueClusteredIndex(string name, string schema = "dbo") {
            return string.Format(
                "CREATE UNIQUE NONCLUSTERED INDEX [UX_{0}_TflKey] ON [{0}] (TflKey ASC);",
                SqlIdentifier(name)
                );
        }

        public string DropUniqueClusteredIndex(string name, string schema = "dbo") {
            return string.Format(
                "DROP INDEX [UX_{0}_TflKey] ON [{0}];",
                SqlIdentifier(name)
                );
        }

        public string WriteTemporary(string name, Field[] fields, AbstractConnection connection, bool useAlias = true) {
            return string.Empty;
        }

    }
}