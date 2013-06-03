using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transformalize.Model;

namespace Transformalize {

    public static class SqlTemplates {
        public static string TruncateTable(string name, string schema = "dbo") {
            return string.Format(@"
                IF EXISTS(
        	        SELECT *
        	        FROM INFORMATION_SCHEMA.TABLES
        	        WHERE TABLE_SCHEMA = '{0}'
        	        AND TABLE_NAME = '{1}'
                )	TRUNCATE TABLE [{0}].[{1}];
            ", schema, name);
        }

        public static string DropTable(string name, string schema = "dbo") {
            return string.Format(@"
                IF EXISTS(
        	        SELECT *
        	        FROM INFORMATION_SCHEMA.TABLES
        	        WHERE TABLE_SCHEMA = '{0}'
        	        AND TABLE_NAME = '{1}'
                )	DROP TABLE [{0}].[{1}];
            ", schema, name);
        }

        public static string CreateTable(string name, IEnumerable<string> defs, IEnumerable<string> keys) {
            var defList = string.Join(", ", defs);
            var keyList = string.Join(", ", keys);
            return string.Format(@"
                CREATE TABLE [{0}](
                    {1},
					CONSTRAINT Pk_{2} PRIMARY KEY (
						{3}
					)
                );
            ", name, defList, name.Replace(" ", string.Empty), keyList);
        }

        public static string CreateTableVariable(string name, IEnumerable<string> defs) {
            var defList = string.Join(", ", defs);
            return string.Format(@"DECLARE {0} AS TABLE({1});", name.StartsWith("@") ? name : "@" + name, defList);
        }

        public static string CreateTableVariable(string name, IDictionary<string, Field> fields) {
            var defs = fields.Keys.Select(key => fields[key]).Select(f => string.Format("[{0}] {1} NOT NULL", f.Alias, f.SqlDataType()));
            return CreateTableVariable(name, defs);
        }

        private static string BatchInsertValues2005(string name, IEnumerable<string> rows, int size) {
            var sqlBuilder = new StringBuilder();
            foreach (var group in rows.Partition(size)) {
                sqlBuilder.Append(string.Format("\r\nINSERT INTO {0} SELECT {1};", name, string.Join(" UNION ALL SELECT ", group)));
            }
            return sqlBuilder.ToString();
        }

        private static string BatchInsertValues2008(string name, IEnumerable<string> rows, int size) {
            var sqlBuilder = new StringBuilder();
            foreach (var group in rows.Partition(size)) {
                var combinedValues = string.Join("),(", group);
                sqlBuilder.Append(string.Format("\r\nINSERT INTO {0} VALUES({1});", name, combinedValues));
            }
            return sqlBuilder.ToString();
        }

        public static string BatchInsertValues(string name, Dictionary<string, Field> fields, IEnumerable<Dictionary<string, object>> rows, int year, int size = 50) {
            return year == 2005 ?
                BatchInsertValues2005(name, RowsToValues(fields, rows), size) :
                BatchInsertValues2008(name, RowsToValues(fields, rows), size);
        }


        private static IEnumerable<string> RowsToValues(Dictionary<string, Field> fields, IEnumerable<Dictionary<string, object>> rows) {
            var lines = new List<string>();
            foreach (var row in rows) {
                var values = new List<string>();
                foreach (var key in row.Keys) {
                    var value = row[key];
                    var field = fields[key];
                    var quote = field.NeedsQuotes() ? "'" : string.Empty;
                    values.Add(string.Format("{0}{1}{0}", quote, value));
                }
                lines.Add(string.Join(",", values));
            }
            return lines;
        }

    }



}
