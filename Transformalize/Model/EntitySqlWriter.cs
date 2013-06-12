using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Model {

    public class EntitySqlWriter : WithLoggingMixin {
        private readonly Entity _entity;

        public EntitySqlWriter(Entity entity) {
            _entity = entity;
        }

        private string BatchInsertValues2005(string name, IDictionary<string, IField> fields, IEnumerable<Row> rows) {
            var sqlBuilder = new StringBuilder();
            foreach (var group in rows.Partition(_entity.InputConnection.BatchInsertSize)) {
                sqlBuilder.Append(string.Format("\r\nINSERT INTO {0}\r\nSELECT {1};", name, string.Join("\r\nUNION ALL SELECT ", RowsToValues(fields, group))));
            }
            return sqlBuilder.ToString();
        }

        private string BatchInsertValues2008(string name, IDictionary<string, IField> fields , IEnumerable<Row> rows) {
            var sqlBuilder = new StringBuilder();
            foreach (var group in rows.Partition(_entity.InputConnection.BatchInsertSize)) {
                sqlBuilder.Append(string.Format("\r\nINSERT INTO {0}\r\nVALUES({1});", name, string.Join("),\r\n(", RowsToValues(fields, @group))));
            }
            return sqlBuilder.ToString();
        }

        public string BatchInsertValues(string name, IDictionary<string, IField> fields, IEnumerable<Row> row) {
            return _entity.InputConnection.Year <= 2005 ?
                BatchInsertValues2005(name, fields, row) :
                BatchInsertValues2008(name, fields, row);
        }


        private static IEnumerable<string> RowsToValues(IDictionary<string, IField> fields, IEnumerable<Row> rows) {
            var lines = new List<string>();
            foreach (var row in rows) {
                var values = new List<string>();
                foreach (var fieldKey in fields.Keys) {
                    var value = row[fieldKey];
                    var field = fields[fieldKey];
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

        public string SelectByKeys(IEnumerable<Row> rows) {
            var writer = new FieldSqlWriter(_entity.Keys).Alias().DataType().NotNull();
            var declareKeys = string.Format(@"DECLARE @KEYS AS TABLE({0});", writer);
            var sql = "SET NOCOUNT ON;\r\n" +
                declareKeys +
                BatchInsertValues("@KEYS", _entity.Keys, rows) + Environment.NewLine +
                SelectJoinedOnKeys();

            Trace(sql);

            return sql;
        }

        public string SelectKeys(bool isRange) {
            const string sqlPattern = @"SELECT {0} FROM [{1}].[{2}] WITH (NOLOCK) WHERE {3} ORDER BY {4};";

            var criteria = string.Format(isRange ? "[{0}] BETWEEN @Begin AND @End" : "[{0}] <= @End", _entity.Version.Name);
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

        public string UpsertSql(IEnumerable<Row> rows)
        {
            var writer = new FieldSqlWriter(_entity.All).ExpandXml().Output().Alias();
            var context = writer.Context();
            var fields = writer.Write(", ", false);
            var fieldsData = string.Concat("d.", fields.Replace(", ", ", d."));
            var definitions = writer.DataType().NotNull().Prepend("\t").Write(",\r\n");

            var table = string.Format("DECLARE @DATA AS TABLE(\r\n{0}\r\n);", definitions);
            var sets = writer.Reload(_entity.Fields).ExpandXml().Output().Alias().Set("o", "d").Write();
            var joins = writer.Reload(_entity.Keys).Alias().Set("o", "d").Write(" AND ");
            var sql = "SET NOCOUNT ON;\r\n" +
                      table + Environment.NewLine +
                      BatchInsertValues("@DATA", context, rows) +
                      Environment.NewLine + Environment.NewLine +
                      string.Format("UPDATE o\r\nSET {0}\r\nFROM [{1}].[{2}] o\r\nINNER JOIN @DATA d ON ({3});", sets, _entity.Schema, _entity.Output, joins) +
                      Environment.NewLine + Environment.NewLine +
                      string.Format("INSERT INTO [{0}].[{1}]({2})\r\nSELECT {3}\r\nFROM @DATA d\r\nLEFT OUTER JOIN [{0}].[{1}] o ON ({4})\r\nWHERE o.{5} IS NULL;", _entity.Schema, _entity.Output, fields, fieldsData, joins, _entity.Keys.First().Key);

            Trace(sql);

            return sql;
        }
    }

}
