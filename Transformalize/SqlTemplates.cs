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

        public static string CreateTableVariable(string name, Entity entity) {
            var keyDefinitions = new FieldSqlWriter(entity.Keys).Alias().DataType().NotNull().Write();
            return string.Format(@"DECLARE {0} AS TABLE({1});", name, keyDefinitions);
        }

        private static string BatchInsertValues2005(string name, Entity entity, IEnumerable<IReadOnlyDictionary<string, object>> keyValues ) {
            var sqlBuilder = new StringBuilder();
            var fields = new FieldSqlWriter(entity.Keys).Name().Write();
            foreach (var group in keyValues.Partition(entity.InputConnection.BatchInsertSize)) {
                sqlBuilder.Append(string.Format("\r\nINSERT INTO {0}({1})\r\nSELECT {2};", name, fields, string.Join(" UNION ALL SELECT ", RowsToValues(entity.Keys, group))));
            }
            return sqlBuilder.ToString();
        }

        private static string BatchInsertValues2008(string name, Entity entity, IEnumerable<IReadOnlyDictionary<string, object>> keyValues) {
            var sqlBuilder = new StringBuilder();
            var fields = new FieldSqlWriter(entity.Keys).Name().Write();
            foreach (var group in keyValues.Partition(entity.InputConnection.BatchInsertSize)) {
                var combinedValues = string.Join("),(", RowsToValues(entity.Keys, group));
                sqlBuilder.Append(string.Format("\r\nINSERT INTO {0}({1}) VALUES({2});", name, fields, combinedValues));
            }
            return sqlBuilder.ToString();
        }


        public static string BatchInsertValues(string name, Entity entity, IEnumerable<IReadOnlyDictionary<string, object>> keyValues ) {
            return entity.InputConnection.Year <= 2005 ?
                BatchInsertValues2005(name, entity, keyValues) :
                BatchInsertValues2008(name, entity, keyValues);
        }


        private static IEnumerable<string> RowsToValues(IReadOnlyDictionary<string, IField> fields, IEnumerable<IReadOnlyDictionary<string, object>> rows) {
            var lines = new List<string>();
            foreach (var row in rows) {
                var values = new List<string>();
                foreach (var key in row.Keys) {
                    var value = row[key];
                    var field = fields[key];
                    values.Add(string.Format("{0}{1}{0}", field.Quote, value));
                }
                lines.Add(string.Join(",", values));
            }
            return lines;
        }

        public static string SelectJoinedOnKeys(Entity entity) {
            const string sqlPattern = "SELECT {0}\r\nFROM [{1}].[{2}] t\r\nINNER JOIN @KEYS k ON ({3});";
            var fields = entity.All.Keys.Select(fieldKey => entity.All[fieldKey]).Where(f => f.FieldType != FieldType.Version && !f.InnerXml.Any()).Select(f => f.SqlWriter.Name().Prepend("t.").ToAlias().Write());
            var xmlFields = entity.All.Keys.Select(fieldKey => entity.All[fieldKey]).Where(f => f.InnerXml.Any()).SelectMany(f => f.InnerXml).Select(kv => kv.Value.SqlWriter.XmlValue().ToAlias().Write());
            var joins = entity.Keys.Keys.Select(keyKey => entity.Keys[keyKey]).Select(f => f.AsJoin("t", "k"));
            return string.Format(sqlPattern, string.Join(", ", fields.Concat(xmlFields)), entity.Schema, entity.Name, string.Join(" AND ", joins));
        }

        public static string SelectByKeys(Entity entity, IEnumerable<IReadOnlyDictionary<string, object>> keyValues ) {
            return
                "SET NOCOUNT ON;\r\n" +
                CreateTableVariable("@KEYS", entity) +
                BatchInsertValues("@KEYS", entity, keyValues) +
                Environment.NewLine +
                SelectJoinedOnKeys(entity);
        }

    }

}
