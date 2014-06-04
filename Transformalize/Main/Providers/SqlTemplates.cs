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
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main.Providers {

    public static class SqlTemplates {

        public static Logger Log = LogManager.GetLogger("tfl");

        public static string TruncateTable(string name, string schema) {
            return string.Format(@"
                IF EXISTS(
        	        SELECT *
        	        FROM INFORMATION_SCHEMA.TABLES
        	        WHERE TABLE_SCHEMA = '{0}'
        	        AND TABLE_NAME = '{1}'
                )	TRUNCATE TABLE [{0}].[{1}];
            ", schema, name);
        }

        public static string DropTable(string name, string schema) {
            return string.Format(@"
                IF EXISTS(
        	        SELECT *
        	        FROM INFORMATION_SCHEMA.TABLES
        	        WHERE TABLE_SCHEMA = '{0}'
        	        AND TABLE_NAME = '{1}'
                )	DROP TABLE [{0}].[{1}];
            ", schema, name);
        }

        public static string Select(Fields fields, string leftTable, string rightTable, AbstractConnection connection, string leftSchema, string rightSchema) {
            var maxDop = connection.MaxDop ? "OPTION (MAXDOP 2);" : ";";
            var sqlPattern = "\r\nSELECT\r\n    {0}\r\nFROM {1} l\r\nINNER JOIN {2} r ON ({3})\r\n" + maxDop;

            var columns = new FieldSqlWriter(fields).Input().Select(connection).Prepend("l.").ToAlias(connection.L, connection.R, true).Write(",\r\n    ");
            var join = new FieldSqlWriter(fields).FieldType(FieldType.MasterKey, FieldType.PrimaryKey).Name(connection.L, connection.R).Input().Set("l", "r").Write(" AND ");

            return string.Format(sqlPattern, columns, SafeTable(leftTable, connection, leftSchema), SafeTable(rightTable, connection, rightSchema), @join);
        }

        public static string Select(Entity entity, AbstractConnection connection) {
            var sql = Select(entity.Fields, entity.Name, connection, entity.Schema, entity.NoLock, entity.Sampled ? 100m : entity.Sample);
            if (entity.Sample > 0m && entity.Sample < 100m && connection.TableSample) {
                entity.Sampled = true;
            }
            return sql;
        }

        public static string Select(Fields fields, string table, AbstractConnection connection, string schema, bool noLock = false, Decimal sample = 100m) {

            var maxDop = connection.MaxDop ? " OPTION (MAXDOP 2)" : string.Empty;
            var withNoLock = noLock && connection.NoLock ? " WITH(NOLOCK)" : string.Empty;

            var tableSample = string.Empty;
            if (sample > 0m && sample < 100m && connection.TableSample) {
                Log.Info("Sample enforced at query level: {0:##} percent.", sample);
                tableSample = string.Format(" TABLESAMPLE ({0:##} PERCENT)", sample);
            }

            var sqlPattern = "\r\nSELECT\r\n    {0}\r\nFROM {1}" + tableSample + withNoLock + maxDop + ";";
            var columns = new FieldSqlWriter(fields).Input().Select(connection).Write(",\r\n    ");

            return string.Format(sqlPattern, columns, SafeTable(table, connection, schema));
        }

        private static string InsertUnionedValues(int size, string name, Fields fields, IEnumerable<Row> rows, AbstractConnection connection) {
            var sqlBuilder = new StringBuilder();
            var safeName = connection.TableVariable ? name : connection.Enclose(name);
            foreach (var group in rows.Partition(size)) {
                sqlBuilder.Append(string.Format("\r\nINSERT INTO {0}\r\nSELECT {1};", safeName, string.Join("\r\nUNION ALL SELECT ", RowsToValues(fields, group))));
            }
            return sqlBuilder.ToString();
        }

        private static string InsertMultipleValues(int size, string name, Fields fields, IEnumerable<Row> rows, AbstractConnection connection) {
            var sqlBuilder = new StringBuilder();
            var safeName = connection.TableVariable ? name : connection.Enclose(name);
            foreach (var group in rows.Partition(size)) {
                sqlBuilder.Append(string.Format("\r\nINSERT INTO {0}\r\nVALUES({1});", safeName, string.Join("),\r\n(", RowsToValues(fields, @group))));
            }
            return sqlBuilder.ToString();
        }

        private static IEnumerable<string> RowsToValues(Fields fields, IEnumerable<Row> rows) {
            foreach (var row in rows) {
                var values = new List<string>();
                foreach (Field field in fields) {
                    var value = row[field.Alias].ToString();
                    var quote = field.Quote();
                    values.Add(
                        quote == string.Empty
                            ? value
                            : string.Concat(quote, value.Replace("'", "''"), quote)
                        );
                }
                yield return string.Join(",", values);
            }
        }

        public static string BatchInsertValues(int size, string name, Fields fields, IEnumerable<Row> rows, AbstractConnection connection) {
            return connection.InsertMultipleRows ?
                InsertMultipleValues(size, name, fields, rows, connection) :
                InsertUnionedValues(size, name, fields, rows, connection);
        }

        private static string SafeTable(string name, AbstractConnection connection, string schema) {
            if (name.StartsWith("@"))
                return name;
            return connection.Schemas && !schema.Equals(string.Empty) ?
                string.Concat(connection.L, schema, string.Format("{0}.{1}", connection.R, connection.L), name, connection.R) :
                string.Concat(connection.L, name, connection.R);
        }
    }
}