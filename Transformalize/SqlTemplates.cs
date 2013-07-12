/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize {

    public static class SqlTemplates {

        private const string CREATE_TABLE_TEMPLATE = @"
CREATE TABLE [{0}].[{1}](
    {2},
    CONSTRAINT [Pk_{3}_{4}] PRIMARY KEY (
        {5}
    ) {6}
);
";

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

        public static string CreateTable(string name, IEnumerable<string> defs, IEnumerable<string> primaryKey, string schema = "dbo", bool ignoreDups = false) {
            var pk = primaryKey.ToArray();
            var defList = string.Join(",\r\n    ", defs);
            var keyName = string.Join("_", pk).Replace("[", string.Empty).Replace("]", string.Empty).Replace(" ", "_");
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

        public static string CreateTableVariable(string name, IDictionary<string, Field> fields, bool useAlias = true) {
            var defs = useAlias ? new FieldSqlWriter(fields).Alias().DataType().Write() : new FieldSqlWriter(fields).Name().DataType().Write();
            return string.Format(@"DECLARE @{0} AS TABLE({1});", name.TrimStart("@".ToCharArray()), defs);
        }

        /// <summary>
        /// Select all the fields from the leftTable, inner joined on the rightTable using leftTable's primary key
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="leftTable"></param>
        /// <param name="rightTable"></param>
        /// <param name="leftSchema">defaults to dbo</param>
        /// <param name="rightSchema">defaults to dbo</param>
        /// <returns>SQL Statement</returns>
        public static string Select(IDictionary<string, Field> fields, string leftTable, string rightTable, string leftSchema = "dbo", string rightSchema = "dbo") {

            const string sqlPattern = "\r\nSELECT\r\n    {0}\r\nFROM {1} l\r\nINNER JOIN {2} r ON ({3})\r\nOPTION (MAXDOP 1);";

            var columns = new FieldSqlWriter(fields).ExpandXml().Input().Select().Prepend("l.").ToAlias().Write(",\r\n    ");
            var join = new FieldSqlWriter(fields).FieldType(FieldType.MasterKey, FieldType.PrimaryKey).Name().Set("l", "r").Write(" AND ");

            return string.Format(sqlPattern, columns, SafeTable(leftTable, leftSchema), SafeTable(rightTable, rightSchema), @join);
        }

        private static string BatchInsertValues2005(int size, string name, IDictionary<string, Field> fields, IEnumerable<Row> rows) {
            var sqlBuilder = new StringBuilder();
            foreach (var group in rows.Partition(size)) {
                sqlBuilder.Append(string.Format("\r\nINSERT INTO {0}\r\nSELECT {1};", name, string.Join("\r\nUNION ALL SELECT ", RowsToValues(fields, group))));
            }
            return sqlBuilder.ToString();
        }

        private static string BatchInsertValues2008(int size, string name, IDictionary<string, Field> fields, IEnumerable<Row> rows) {
            var sqlBuilder = new StringBuilder();
            foreach (var group in rows.Partition(size)) {
                sqlBuilder.Append(string.Format("\r\nINSERT INTO {0}\r\nVALUES({1});", name, string.Join("),\r\n(", RowsToValues(fields, @group))));
            }
            return sqlBuilder.ToString();
        }

        private static IEnumerable<string> RowsToValues(IDictionary<string, Field> fields, IEnumerable<Row> rows) {
            foreach (var row in rows) {
                var values = new List<string>();
                foreach (var pair in fields) {
                    var value = row[pair.Key].ToString();
                    values.Add(
                        pair.Value.Quote == string.Empty
                        ? value
                        : string.Concat(pair.Value.Quote, value.Replace("'", "''"), pair.Value.Quote)
                    );
                }
                yield return string.Join(",", values);
            }
        }

        public static string BatchInsertValues(int size, string name, IDictionary<string, Field> fields, IEnumerable<Row> rows, int year) {
            return year <= 2005 ?
                BatchInsertValues2005(size, name, fields, rows) :
                BatchInsertValues2008(size, name, fields, rows);
        }

        private static string SafeTable(string name, string schema = "dbo") {
            if (name.StartsWith("@"))
                return name;
            return schema.Equals("dbo", StringComparison.OrdinalIgnoreCase) ?
                string.Concat("[", name, "]") :
                string.Concat("[", schema, "].[", name, "]");
        }
        
    }
}
