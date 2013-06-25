using System;
using System.Collections.Generic;
using System.Linq;
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
                      SqlTemplates.CreateTableVariable(KEYS_TABLE_VARIABLE, context) +
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

            foreach (var key in _entity.PrimaryKey.Keys) {
                var field = _entity.PrimaryKey[key];
                var name = field.Name;
                var alias = field.Alias;
                selectKeys.Add(alias.Equals(name) ? string.Concat("[", name, "]") : string.Format("{0} = [{1}]", alias, name));
                orderByKeys.Add(string.Concat("[", name, "]"));
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

                _upsertSqlStatement = Environment.NewLine + "SET NOCOUNT OFF;\r\n" + Environment.NewLine +
                      string.Format("UPDATE o\r\nSET {0}\r\nFROM [{1}].[{2}] o\r\nINNER JOIN @DATA d ON ({3});", sets, _entity.Schema, _entity.Output, joins) +
                      Environment.NewLine + (
                        doInsert ?
                            Environment.NewLine + string.Format("INSERT INTO [{0}].[{1}]({2})\r\nSELECT {3}\r\nFROM @DATA d\r\nLEFT OUTER JOIN [{0}].[{1}] o ON ({4})\r\nWHERE o.{5} IS NULL;", _entity.Schema, _entity.Output, fields, fieldsData, joins, _entity.PrimaryKey.First().Key) :
                            string.Empty
                      );
            }
            return _upsertSqlStatement;

        }

        public string UpdateSql(IEnumerable<Row> @group) {
            return UpsertSql(@group, doInsert: false);
        }
    }

}
