using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Model {

    public class EntitySqlWriter : WithLoggingMixin {
        private readonly Entity _entity;

        public EntitySqlWriter(Entity entity)
        {
            _entity = entity;
        }

        public string CreateTableVariable(string name) {
            var keyDefinitions = new FieldSqlWriter(_entity.Keys).Alias().DataType().NotNull().Write();
            return string.Format(@"DECLARE {0} AS TABLE({1});", name, keyDefinitions);
        }

        private string BatchInsertValues2005(string name, IEnumerable<Row> rows) {
            var sqlBuilder = new StringBuilder();
            var fields = new FieldSqlWriter(_entity.Keys).Name().Write();
            foreach (var group in rows.Partition(_entity.InputConnection.BatchInsertSize)) {
                sqlBuilder.Append(string.Format("\r\nINSERT INTO {0}({1})\r\nSELECT {2};", name, fields, string.Join(" UNION ALL SELECT ", RowsToValues(_entity.Keys, group))));
            }
            return sqlBuilder.ToString();
        }

        private string BatchInsertValues2008(string name, IEnumerable<Row> rows) {
            var sqlBuilder = new StringBuilder();
            var fields = new FieldSqlWriter(_entity.Keys).Name().Write();
            foreach (var group in rows.Partition(_entity.InputConnection.BatchInsertSize)) {
                var combinedValues = string.Join("),(", RowsToValues(_entity.Keys, group));
                sqlBuilder.Append(string.Format("\r\nINSERT INTO {0}({1}) VALUES({2});", name, fields, combinedValues));
            }
            return sqlBuilder.ToString();
        }

        public string BatchInsertValues(string name, IEnumerable<Row> row) {
            return _entity.InputConnection.Year <= 2005 ?
                BatchInsertValues2005(name, row) :
                BatchInsertValues2008(name, row);
        }


        private static IEnumerable<string> RowsToValues(IReadOnlyDictionary<string, IField> fields, IEnumerable<Row> rows) {
            var lines = new List<string>();
            foreach (var row in rows) {
                var values = new List<string>();
                foreach (var key in row.Columns) {
                    var value = row[key];
                    var field = fields[key];
                    values.Add(string.Format("{0}{1}{0}", field.Quote, value));
                }
                lines.Add(string.Join(",", values));
            }
            return lines;
        }

        public string SelectJoinedOnKeys() {
            const string sqlPattern = "SELECT {0}\r\nFROM [{1}].[{2}] t\r\nINNER JOIN @KEYS k ON ({3});";
            var fields = _entity.All.Keys.Select(fieldKey => _entity.All[fieldKey]).Where(f => f.FieldType != FieldType.Version && !f.InnerXml.Any()).Select(f => f.SqlWriter.Name().Prepend("t.").ToAlias().Write());
            var xmlFields = _entity.All.Keys.Select(fieldKey => _entity.All[fieldKey]).Where(f => f.InnerXml.Any()).SelectMany(f => f.InnerXml).Select(kv => kv.Value.SqlWriter.XmlValue().ToAlias().Write());
            var joins = _entity.Keys.Keys.Select(keyKey => _entity.Keys[keyKey]).Select(f => f.AsJoin("t", "k"));
            var sql = string.Format(sqlPattern, string.Join(", ", fields.Concat(xmlFields)), _entity.Schema, _entity.Name, string.Join(" AND ", joins));

            Trace(sql);

            return sql;
        }

        public string SelectByKeys(IEnumerable<Row> keyValues) {
            var sql = "SET NOCOUNT ON;\r\n" +
                CreateTableVariable("@KEYS") +
                BatchInsertValues("@KEYS", keyValues) +
                Environment.NewLine +
                SelectJoinedOnKeys();

            Trace(sql);

            return sql;
        }

        public string SelectKeys(bool isRange) {
            const string sqlPattern = @"SELECT {0} FROM [{1}].[{2}] WITH (NOLOCK) WHERE {3} ORDER BY {4};";

            var criteria = string.Format(isRange ? "[{0}] BETWEEN @Start AND @End" : "[{0}] <= @End", _entity.Version.Name);
            var orderByKeys = new List<string>();
            var selectKeys = new List<string>();

            foreach (var key in _entity.Keys.Keys) {
                var field = _entity.Keys[key];
                var name = field.Name;
                var alias = field.Alias;
                selectKeys.Add(alias.Equals(name) ? string.Concat("[", name, "]") : string.Format("{0} = [{1}]", alias, name));
                orderByKeys.Add(string.Concat("[", name, "]"));
            }

            return string.Format(sqlPattern, string.Join(", ", selectKeys), _entity.Schema, _entity.Name, criteria, string.Join(", ", orderByKeys));
        }

    }

}
