using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Transformalize.Model {
    public class Input {

        private Dictionary<string, IField> _fields;
        public Dictionary<string, Entity> Entities = new Dictionary<string, Entity>();
        public List<Join> Joins = new List<Join>();

        private Dictionary<string, IField> Fields() {
            if (_fields != null)
                return _fields;

            _fields = new Dictionary<string, IField>();
            foreach (var entityKey in Entities.Keys) {
                foreach (var fieldKey in Entities[entityKey].All.Keys) {
                    var field = Entities[entityKey].All[fieldKey];
                    if (field.Xml.Any()) {
                        foreach (var xmlKey in field.Xml.Keys) {
                            _fields[xmlKey] = field.Xml[xmlKey];
                        }
                    }
                    else {
                        _fields[fieldKey] = field;
                    }

                }
            }
            return _fields;
        }

        public SortedDictionary<string, IField> OutputFields() {
            var result = new SortedDictionary<string, IField>();
            var fields = Fields();
            foreach (var fieldKey in fields.Where(f=>f.Value.Output).Select(f=>f.Key)) {
                result[fieldKey] = fields[fieldKey];
            }
            return result;
        }

        public string CreateSql(string outputName) {

            var sqlBuilder = new StringBuilder(Environment.NewLine);
            sqlBuilder.AppendFormat("CREATE TABLE [dbo].[{0}](\r\n", outputName);

            var fields = OutputFields();
            var rowVersionKey = outputName + "RowVersion";
            var timeKeyKey = outputName + "TimeKey";
            var loadDateKey = outputName + "LoadDate";

            fields.Add(rowVersionKey, new Field() { Alias = rowVersionKey, Type = "System.RowVersion" });
            fields.Add(timeKeyKey, new Field() { Alias = timeKeyKey, Type = "System.Int32" });
            fields.Add(loadDateKey, new Field() { Alias = loadDateKey, Type = "System.DateTime" });

            foreach (var fieldKey in fields.Keys) {
                sqlBuilder.AppendFormat("[{0}] {1} NOT NULL,\r\n", fields[fieldKey].Alias, fields[fieldKey].SqlDataType());
            }

            sqlBuilder.AppendFormat("CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED (\r\n", outputName);

            var entity = Entities[Entities.Keys.First()];
            var keys = entity.Keys.Keys.Select(key => string.Format("[{0}] ASC\r\n", entity.Keys[key].Alias));
            sqlBuilder.AppendFormat(string.Join(", ", keys));

            sqlBuilder.Append(") WITH (IGNORE_DUP_KEY = ON));");
            return sqlBuilder.ToString();
        }
    }
}