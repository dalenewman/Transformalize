#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Transformalize.Extensions;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main.Providers
{
    public static class SqlTemplates
    {
        public static string TruncateTable(string name, string schema = "dbo")
        {
            return string.Format(@"
                IF EXISTS(
        	        SELECT *
        	        FROM INFORMATION_SCHEMA.TABLES
        	        WHERE TABLE_SCHEMA = '{0}'
        	        AND TABLE_NAME = '{1}'
                )	TRUNCATE TABLE [{0}].[{1}];
            ", schema, name);
        }

        public static string DropTable(string name, string schema = "dbo")
        {
            return string.Format(@"
                IF EXISTS(
        	        SELECT *
        	        FROM INFORMATION_SCHEMA.TABLES
        	        WHERE TABLE_SCHEMA = '{0}'
        	        AND TABLE_NAME = '{1}'
                )	DROP TABLE [{0}].[{1}];
            ", schema, name);
        }

        public static string Select(IFields fields, string leftTable, string rightTable, AbstractProvider provider, string leftSchema = "dbo", string rightSchema = "dbo")
        {
            var maxDop = provider.Supports.MaxDop ? "OPTION (MAXDOP 2);" : ";";
            var sqlPattern = "\r\nSELECT\r\n    {0}\r\nFROM {1} l\r\nINNER JOIN {2} r ON ({3})\r\n" + maxDop;

            var columns = new FieldSqlWriter(fields).ExpandXml().Input().Select(provider).Prepend("l.").ToAlias(provider, true).Write(",\r\n    ");
            var join = new FieldSqlWriter(fields).FieldType(FieldType.MasterKey, FieldType.PrimaryKey).Name(provider).Set("l", "r").Write(" AND ");

            return string.Format(sqlPattern, columns, SafeTable(leftTable, provider, leftSchema), SafeTable(rightTable, provider, rightSchema), @join);
        }

        private static string InsertUnionedValues(int size, string name, Field[] fields, IEnumerable<Row> rows, AbstractConnection connection)
        {
            var sqlBuilder = new StringBuilder();
            var safeName = connection.Provider.Supports.TableVariable ? name : connection.Provider.Enclose(name);
            foreach (var group in rows.Partition(size))
            {
                sqlBuilder.Append(string.Format("\r\nINSERT INTO {0}\r\nSELECT {1};", safeName, string.Join("\r\nUNION ALL SELECT ", RowsToValues(fields, group))));
            }
            return sqlBuilder.ToString();
        }

        private static string InsertMultipleValues(int size, string name, Field[] fields, IEnumerable<Row> rows, AbstractConnection connection)
        {
            var sqlBuilder = new StringBuilder();
            var safeName = connection.Provider.Supports.TableVariable ? name : connection.Provider.Enclose(name);
            foreach (var group in rows.Partition(size))
            {
                sqlBuilder.Append(string.Format("\r\nINSERT INTO {0}\r\nVALUES({1});", safeName, string.Join("),\r\n(", RowsToValues(fields, @group))));
            }
            return sqlBuilder.ToString();
        }

        private static IEnumerable<string> RowsToValues(Field[] fields, IEnumerable<Row> rows)
        {
            var orderedFields = new FieldSqlWriter(fields).ToArray();
            foreach (var row in rows)
            {
                var values = new List<string>();
                foreach (var field in orderedFields)
                {
                    var value = row[field.Alias].ToString();
                    values.Add(
                        field.Quote == string.Empty
                            ? value
                            : string.Concat(field.Quote, value.Replace("'", "''"), field.Quote)
                        );
                }
                yield return string.Join(",", values);
            }
        }

        public static string BatchInsertValues(int size, string name, Field[] fields, IEnumerable<Row> rows, AbstractConnection connection)
        {
            return connection.Provider.Supports.InsertMultipleRows ?
                       InsertMultipleValues(size, name, fields, rows, connection) :
                       InsertUnionedValues(size, name, fields, rows, connection);
        }

        private static string SafeTable(string name, AbstractProvider provider, string schema = "dbo")
        {
            if (name.StartsWith("@"))
                return name;
            return schema.Equals("dbo", StringComparison.OrdinalIgnoreCase) ?
                       string.Concat(provider.L, name, provider.R) :
                       string.Concat(provider.L, schema, string.Format("{0}.{1}", provider.R, provider.L), name, provider.R);
        }
    }
}