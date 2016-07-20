#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Data;
using System.Linq;
using System.Text;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Extensions;

namespace Pipeline.Provider.Ado.Ext {
    public static class AdoExtensions {

        public static string SqlControlTableName(this OutputContext c) {
            return Utility.Identifier(c.Process.Name) + "Control";
        }

        static string DefaultValue(Field field, IConnectionFactory cf) {

            if (field.Default == null)
                return "NULL";

            var d = field.Default == Constants.DefaultSetting ? Constants.StringDefaults()[field.Type] : field.Default;

            if (AdoConstants.StringTypes.Any(t => t == field.Type)) {
                return "'" + d + "'";
            }

            if (!field.Type.StartsWith("bool", StringComparison.Ordinal))
                return d;

            if (cf.AdoProvider == AdoProvider.PostgreSql) {
                return d.Equals("true", StringComparison.OrdinalIgnoreCase) ? "true" : "false";
            }
            return d.Equals("true", StringComparison.OrdinalIgnoreCase) ? "1" : "0";
        }

        public static string SqlCreateOutputUniqueIndex(this OutputContext c, IConnectionFactory cf) {
            var pk = c.Entity.GetAllFields().Where(f => f.PrimaryKey).Select(f => f.FieldName()).ToArray();
            var indexName = ("UX_" + Utility.Identifier(c.Entity.OutputTableName(c.Process.Name) + "_" + SqlKeyName(pk))).Left(128);
            var sql = $"CREATE UNIQUE INDEX {cf.Enclose(indexName)} ON {cf.Enclose(c.Entity.OutputTableName(c.Process.Name))} ({string.Join(",", pk.Select(cf.Enclose))});";
            c.Debug(() => sql);
            return sql;
        }

        public static string SqlSelectOutputSchema(this OutputContext c, IConnectionFactory cf) {
            var table = $"{cf.Enclose(c.Entity.OutputTableName(c.Process.Name))}";
            var sql = cf.AdoProvider == AdoProvider.SqlServer ?
                $"SELECT TOP 0 * FROM {table};" :
                $"SELECT * FROM {table} LIMIT 0;";
            c.Debug(() => sql);
            return sql;
        }

        public static string SchemaPrefix(this IContext c, IConnectionFactory f) {
            return c.Entity.Schema == string.Empty ? string.Empty : f.Enclose(c.Entity.Schema) + ".";
        }

        public static string SqlSelectInput(this InputContext c, Field[] fields, IConnectionFactory cf, Func<IConnectionFactory, string> orderBy) {
            var fieldList = string.Join(",", fields.Select(f => cf.Enclose(f.Name)));
            var rowNumber = string.Empty;

            if (c.Entity.IsPageRequest()) {
                if (cf.AdoProvider == AdoProvider.SqlServer) {
                    if (c.Entity.Order.Any()) {
                        rowNumber = $", ROW_NUMBER() OVER ({orderBy(cf)}) AS TflRow";
                    } else {
                        var keys = string.Join(", ", fields.Where(f => f.PrimaryKey).Select(f => cf.Enclose(f.Name)));
                        if (string.IsNullOrEmpty(keys)) {
                            keys = fields.First(f => f.Input).Name;
                        }
                        rowNumber = $", ROW_NUMBER() OVER (ORDER BY {keys}) AS TflRow";
                    }

                }
            }

            var sql = $@"
                SELECT {fieldList}{rowNumber}
                FROM {SqlInputName(c, cf)} {(cf.AdoProvider == AdoProvider.SqlServer && c.Entity.NoLock ? " WITH (NOLOCK) " : string.Empty)}
                {(c.Entity.Filter.Any() ? " WHERE " + c.ResolveFilter(cf) : string.Empty)}
            ";

            if (!c.Entity.IsPageRequest() && orderBy != null) {
                sql += orderBy(cf);
            }

            return sql;
        }

        public static string SqlInputName(this InputContext c, IConnectionFactory cf) {
            return SchemaPrefix(c, cf) + cf.Enclose(c.Entity.Name);
        }

        public static string SqlSelectInputWithMinVersion(this InputContext c, Field[] fields, IConnectionFactory cf, Func<IConnectionFactory, string> orderBy) {
            var coreSql = SqlSelectInput(c, fields, cf, null);
            var hasWhere = coreSql.Contains(" WHERE ");
            var version = c.Entity.GetVersionField();
            var sql = $@"{coreSql} {(hasWhere ? " AND " : " WHERE ")} {cf.Enclose(version.Name)} {(c.Entity.Overlap ? ">=" : ">")} @MinVersion";
            sql += orderBy(cf);
            return sql;
        }

        public static string SqlCreateOutput(this OutputContext c, IConnectionFactory cf) {
            var columnsAndDefinitions = string.Join(",", c.GetAllEntityOutputFields().Select(f => cf.Enclose(f.FieldName()) + " " + cf.SqlDataType(f) + " NOT NULL"));
            var sql = $"CREATE TABLE {cf.Enclose(c.Entity.OutputTableName(c.Process.Name))}({columnsAndDefinitions}, ";
            if (cf.AdoProvider == AdoProvider.SqLite) {
                sql += $"PRIMARY KEY ({cf.Enclose(c.Entity.TflKey().FieldName())} ASC));";
            } else {
                sql += $"CONSTRAINT {Utility.Identifier("pk_" + c.Entity.OutputTableName(c.Process.Name) + "_tflkey")} PRIMARY KEY ({cf.Enclose(c.Entity.TflKey().FieldName())}));";
            }
            c.Debug(() => sql);
            return sql;
        }

        public static string SqlSchemaPrefix(this IContext c, IConnectionFactory cf) {
            return c.Entity.Schema == string.Empty ? string.Empty : cf.Enclose(c.Entity.Schema) + ".";
        }

        public static string SqlInsertIntoOutput(this OutputContext c, IConnectionFactory cf) {
            var fields = c.OutputFields.ToArray();
            var parameters = string.Join(",", fields.Select(f => "@" + f.FieldName()));
            var sql = $"INSERT INTO {cf.Enclose(c.Entity.OutputTableName(c.Process.Name))} VALUES({parameters});";
            c.Debug(() => sql);
            return sql;
        }

        public static string SqlUpdateOutput(this OutputContext c, IConnectionFactory cf) {
            var fields = c.Entity.GetAllFields().Where(f => f.Output).ToArray();
            var sets = string.Join(",", fields.Where(f => !f.PrimaryKey && f.Name != Constants.TflKey).Select(f => f.FieldName()).Select(n => cf.Enclose(n) + " = @" + n));
            var criteria = string.Join(" AND ", fields.Where(f => f.PrimaryKey).Select(f => f.FieldName()).Select(n => cf.Enclose(n) + " = @" + n));
            var sql = $"UPDATE {cf.Enclose(c.Entity.OutputTableName(c.Process.Name))} SET {sets} WHERE {criteria};";
            c.Debug(() => sql);
            return sql;
        }

        public static string SqlUpdateCalculatedFields(this OutputContext c, Process original, IConnectionFactory cnf) {
            var master = original.Entities.First(e => e.IsMaster);
            var fields = c.Entity.CalculatedFields.Where(f => f.Output && f.Name != Constants.TflKey).ToArray();
            var sets = string.Join(",", fields.Select(f => cnf.Enclose(original.CalculatedFields.First(cf => cf.Name == f.Name).FieldName()) + " = @" + f.FieldName()));
            var key = c.Entity.TflKey().FieldName();
            var sql = $"UPDATE {cnf.Enclose(master.OutputTableName(original.Name))} SET {sets} WHERE {cnf.Enclose(key)} = @{key};";
            c.Debug(() => sql);
            return sql;
        }

        public static string SqlDeleteOutput(this OutputContext c, IConnectionFactory cf, int batchId) {
            var deletedValue = cf.AdoProvider == AdoProvider.PostgreSql ? "true" : "1";
            var criteria = string.Join(" AND ", c.Entity.GetPrimaryKey().Select(f => f.FieldName()).Select(n => cf.Enclose(n) + " = @" + n));
            var sql = $"UPDATE {cf.Enclose(c.Entity.OutputTableName(c.Process.Name))} SET {cf.Enclose(c.Entity.TflDeleted().FieldName())} = {deletedValue} WHERE {criteria}";
            c.Debug(() => sql);
            return sql;
        }

        public static string SqlDropOutput(this OutputContext c, IConnectionFactory cf) {
            var cascade = cf.AdoProvider == AdoProvider.PostgreSql ? " CASCADE" : string.Empty;
            var sql = $"DROP TABLE {cf.Enclose(c.Entity.OutputTableName(c.Process.Name))}{cascade};";
            c.Debug(() => sql);
            return sql;
        }

        public static string SqlDropOutputView(this OutputContext c, IConnectionFactory cf) {
            var sql = $"DROP VIEW {cf.Enclose(c.Entity.OutputViewName(c.Process.Name))};";
            c.Debug(() => sql);
            return sql;
        }

        public static string SqlDropOutputViewAsTable(this OutputContext c, IConnectionFactory cf) {
            var sql = $"DROP TABLE {cf.Enclose(c.Entity.OutputViewName(c.Process.Name))};";
            c.Debug(() => sql);
            return sql;
        }

        public static string SqlDropControl(this OutputContext c, IConnectionFactory cf) {
            var sql = $"DROP TABLE {cf.Enclose(SqlControlTableName(c))};";
            c.Debug(() => sql);
            return sql;
        }

        public static string SqlDeleteEntityFromControl(this OutputContext c, IConnectionFactory cf) {
            var sql = $"DELETE FROM {cf.Enclose(SqlControlTableName(c))} WHERE Entity = @Entity";
            c.Debug(() => sql);
            return sql;
        }

        public static string SqlCount(this OutputContext c, IConnectionFactory cf) {
            return $"SELECT COUNT(*) FROM {cf.Enclose(c.Entity.OutputTableName(c.Process.Name))};";
        }

        public static string SqlControlLastBatchId(this OutputContext c, IConnectionFactory cf) {
            var sql = $"SELECT COALESCE(MAX({cf.Enclose("BatchId")}),0) FROM {cf.Enclose(SqlControlTableName(c))};";
            c.Debug(() => sql);
            return sql;
        }

        public static string SqlControlStartBatch(this OutputContext c, IConnectionFactory cf) {
            var sql = $@"INSERT INTO {cf.Enclose(SqlControlTableName(c))}({cf.Enclose("BatchId")},{cf.Enclose("Entity")},{cf.Enclose("Inserts")},{cf.Enclose("Updates")},{cf.Enclose("Deletes")},{cf.Enclose("Start")},{cf.Enclose("End")}) VALUES(@BatchId,@Entity,0,0,0,@Now,null);";
            c.Debug(() => sql);
            return sql;
        }

        public static string SqlControlEndBatch(this OutputContext c, IConnectionFactory cf) {
            var sql = $"UPDATE {cf.Enclose(SqlControlTableName(c))} SET {cf.Enclose("Inserts")} = @Inserts, {cf.Enclose("Updates")} = @Updates, {cf.Enclose("Deletes")} = @Deletes, {cf.Enclose("End")} = @Now WHERE {cf.Enclose("Entity")} = @Entity AND {cf.Enclose("BatchId")} = @BatchId;";
            c.Debug(() => sql);
            return sql;
        }

        public static string SqlCreateControl(this OutputContext c, IConnectionFactory cf) {
            var sql = $@"
                CREATE TABLE {cf.Enclose(SqlControlTableName(c))}(
                    {cf.Enclose("BatchId")} INTEGER NOT NULL,
                    {cf.Enclose("Entity")} {(cf.AdoProvider == AdoProvider.SqlServer ? "N" : string.Empty)}VARCHAR(128) NOT NULL,
                    {cf.Enclose("Inserts")} BIGINT NOT NULL,
                    {cf.Enclose("Updates")} BIGINT NOT NULL,
                    {cf.Enclose("Deletes")} BIGINT NOT NULL,
                    {cf.Enclose("Start")} {(cf.AdoProvider == AdoProvider.PostgreSql ? "TIMESTAMP" : "DATETIME")} NOT NULL,
                    {cf.Enclose("End")} {(cf.AdoProvider == AdoProvider.PostgreSql ? "TIMESTAMP" : "DATETIME")},
                    CONSTRAINT PK_{Utility.Identifier(SqlControlTableName(c))}_BatchId PRIMARY KEY ({cf.Enclose("BatchId")}, {cf.Enclose("Entity")})
                );";
            c.Debug(() => sql);
            return sql;
        }

        public static string SqlCreateOutputView(this OutputContext c, IConnectionFactory cf) {
            var columnNames = string.Join(",", c.GetAllEntityOutputFields().Select(f => cf.Enclose(f.FieldName()) + " AS " + cf.Enclose(f.Alias)));
            var sql = $@"CREATE VIEW {cf.Enclose(c.Entity.OutputViewName(c.Process.Name))} AS SELECT {columnNames} FROM {cf.Enclose(c.Entity.OutputTableName(c.Process.Name))};";
            c.Debug(() => sql);
            return sql;
        }

        public static string SqlDropStarView(this OutputContext c, IConnectionFactory cf) {
            var sql = $"DROP VIEW {cf.Enclose(c.Process.Star)};";
            c.Debug(() => sql);
            return sql;
        }

        public static string SqlCreateStarView(this IContext c, IConnectionFactory cf) {
            var starFields = c.Process.GetStarFields().ToArray();
            var master = c.Process.Entities.First(e => e.IsMaster);
            var masterAlias = Utility.GetExcelName(master.Index);
            var masterNames = string.Join(",", starFields[0].Select(f => masterAlias + "." + cf.Enclose(f.FieldName()) + " AS " + cf.Enclose(f.Alias)));
            var slaveNames = string.Join(",", starFields[1].Select(f => "COALESCE(" + Utility.GetExcelName(f.EntityIndex) + "." + cf.Enclose(f.FieldName()) + ", " + DefaultValue(f, cf) + ") AS " + cf.Enclose(f.Alias)));

            var builder = new StringBuilder();

            foreach (var entity in c.Process.Entities.Where(e => !e.IsMaster)) {
                builder.AppendFormat("LEFT OUTER JOIN {0} {1} ON (", cf.Enclose(entity.OutputTableName(c.Process.Name)), Utility.GetExcelName(entity.Index));

                var relationship = entity.RelationshipToMaster.First();

                foreach (var join in relationship.Join.ToArray()) {
                    var leftField = c.Process.GetEntity(relationship.LeftEntity).GetField(join.LeftField);
                    var rightField = entity.GetField(join.RightField);
                    builder.AppendFormat(
                        "{0}.{1} = {2}.{3} AND ",
                        masterAlias,
                        cf.Enclose(leftField.FieldName()),
                        Utility.GetExcelName(entity.Index),
                        cf.Enclose(rightField.FieldName())
                    );
                }

                if (entity.Delete) {
                    builder.Append($"{cf.Enclose(Utility.GetExcelName(entity.Index))}.{cf.Enclose(entity.TflDeleted().FieldName())} = 0");
                } else {
                    builder.Remove(builder.Length - 5, 5);
                }

                builder.AppendLine(") ");
            }

            var sql = $"CREATE VIEW {cf.Enclose(c.Process.Star)} AS SELECT {masterNames}{(slaveNames == string.Empty ? string.Empty : "," + slaveNames)} FROM {cf.Enclose(master.OutputTableName(c.Process.Name))} {masterAlias} {builder};";
            c.Debug(() => sql);
            return sql;
        }

        static string SqlKeyName(string[] pk) {
            return Utility.Identifier(
                string.Join("_", pk)
            );
        }

        public static IDbDataParameter AddParameter(this IDbCommand cmd, string name, object value) {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value;
            p.Direction = ParameterDirection.Input;
            return p;
        }


    }
}
