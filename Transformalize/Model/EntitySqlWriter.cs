using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Model {

    public class EntitySqlWriter : WithLoggingMixin {
        private string _upsertSqlStatement;
        private readonly Entity _entity;
        private const string KEYS_TABLE_VARIABLE = "@KEYS";

        public EntitySqlWriter(Entity entity) {
            _entity = entity;
        }

        public string SelectByKeys(IEnumerable<Row> rows) {
            var context = new FieldSqlWriter(_entity.PrimaryKey).Context();
            var sql = "SET NOCOUNT ON;\r\n" +
                      SqlTemplates.CreateTableVariable(KEYS_TABLE_VARIABLE, context, false) +
                      SqlTemplates.BatchInsertValues(50, KEYS_TABLE_VARIABLE, context, rows, _entity.InputConnection.Year) + Environment.NewLine +
                      SqlTemplates.Select(_entity.All, _entity.Name, KEYS_TABLE_VARIABLE);

            Trace(sql);

            return sql;
        }

        public string SelectKeys(bool isRange) {
            const string sqlPattern = @"SELECT {0} FROM [{1}].[{2}] WITH (NOLOCK) WHERE {3} ORDER BY {4};";

            var criteria = string.Format(isRange ? "[{0}] BETWEEN @Begin AND @End" : "[{0}] <= @End", _entity.Version.Name);
            var orderByKeys = new List<string>();
            var selectKeys = new List<string>();

            foreach (var pair in _entity.PrimaryKey) {
                selectKeys.Add(pair.Value.Alias.Equals(pair.Value.Name) ? string.Concat("[", pair.Value.Name, "]") : string.Format("{0} = [{1}]", pair.Value.Alias, pair.Value.Name));
                orderByKeys.Add(string.Concat("[", pair.Value.Name, "]"));
            }

            return string.Format(sqlPattern, string.Join(", ", selectKeys), _entity.Schema, _entity.Name, criteria, string.Join(", ", orderByKeys));
        }

        public string UpsertSql(IEnumerable<Row> rows, bool doInsert = true) {
            var context = new FieldSqlWriter(_entity.All).ExpandXml().Output().Context();
            var table = SqlTemplates.CreateTableVariable("@DATA", context);
            var sql = "SET NOCOUNT ON;\r\n" +
                      table + Environment.NewLine +
                      SqlTemplates.BatchInsertValues(50, "@DATA", context, rows, _entity.OutputConnection.Year) +
                      UpsertSqlStatement(doInsert);

            Trace(sql);

            return sql;
        }

        private string UpsertSqlStatement(bool doInsert = true) {

            if (_upsertSqlStatement == null) {
                var writer = new FieldSqlWriter(_entity.All).ExpandXml().Output();
                var fields = writer.Alias().Write(", ", false);
                var fieldsData = string.Concat("d.", fields.Replace(", ", ", d."));
                var sets = writer.Reload(_entity.Fields).ExpandXml().Input().Output().Alias().Set("o", "d").Write();
                var joins = writer.Reload(_entity.PrimaryKey).Alias().Set("o", "d").Write(" AND ");

                var sqlBuilder = new StringBuilder(Environment.NewLine);
                sqlBuilder.AppendLine("SET NOCOUNT OFF;");
                sqlBuilder.AppendLine(string.Empty);
                sqlBuilder.AppendFormat("WITH diff AS (\r\n    SELECT {0} FROM @DATA\r\n    EXCEPT\r\n    SELECT {0} FROM [{1}].[{2}]\r\n)   ", fields, _entity.Schema, _entity.OutputName());
                sqlBuilder.AppendFormat("UPDATE o\r\n    SET {0}, TflId = @TflId\r\n    FROM [{1}].[{2}] o\r\n    INNER JOIN diff d ON ({3});", sets, _entity.Schema, _entity.OutputName(), joins);
                sqlBuilder.AppendLine(string.Empty);

                if (doInsert) {
                    sqlBuilder.AppendLine(string.Empty);
                    sqlBuilder.AppendFormat("INSERT INTO [{0}].[{1}]({2}, TflId)\r\nSELECT {3}, @TflId\r\nFROM @DATA d\r\nLEFT OUTER JOIN [{0}].[{1}] o ON ({4})\r\nWHERE o.{5} IS NULL;", _entity.Schema, _entity.OutputName(), fields, fieldsData, joins, _entity.PrimaryKey.First().Key);
                }

                _upsertSqlStatement = sqlBuilder.ToString();

            }
            return _upsertSqlStatement;

        }

        public string UpdateSql(IEnumerable<Row> @group) {
            return UpsertSql(@group, doInsert: false);
        }
    }

}
