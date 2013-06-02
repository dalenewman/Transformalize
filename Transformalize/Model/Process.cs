using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transformalize.Configuration;

namespace Transformalize.Model {

    public class Process {

        const string ROW_VERSION_KEY = "RowVersion";
        const string TIME_KEY_KEY = "TimeKey";
        const string LOAD_DATE_KEY = "LoadDate";

        private Dictionary<string, IField> _fields;

        public string Name;
        public Dictionary<string, Connection> Connections = new Dictionary<string, Connection>();
        public string Output;
        public string Time;
        public Connection OutputConnection;
        public Dictionary<string, Entity> Entities = new Dictionary<string, Entity>();
        public List<Join> Joins = new List<Join>();

        public string TruncateOutputSql() {
            return SqlTemplates.TruncateSql(Output);
        }

        public string DropOutputSql() {
            return SqlTemplates.DropSql(Output);
        }

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
            foreach (var fieldKey in fields.Where(f => f.Value.Output).Select(f => f.Key)) {
                result[fieldKey] = fields[fieldKey];
            }
            return result;
        }

        public string CreateOutputSql() {

            var sqlBuilder = new StringBuilder(Environment.NewLine);
            sqlBuilder.AppendFormat("CREATE TABLE [dbo].[{0}](\r\n", Output);

            var fields = OutputFields();

            fields.Add(ROW_VERSION_KEY, new Field() { Alias = ROW_VERSION_KEY, Type = "System.RowVersion" });
            fields.Add(TIME_KEY_KEY, new Field() { Alias = TIME_KEY_KEY, Type = "System.Int32" });
            fields.Add(LOAD_DATE_KEY, new Field() { Alias = LOAD_DATE_KEY, Type = "System.DateTime" });

            foreach (var fieldKey in fields.Keys) {
                sqlBuilder.AppendFormat("[{0}] {1} NOT NULL,\r\n", fields[fieldKey].Alias, fields[fieldKey].SqlDataType());
            }

            sqlBuilder.AppendFormat("CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED (\r\n", Output);

            var entity = Entities[Entities.Keys.First()];
            var keys = entity.Keys.Keys.Select(key => string.Format("[{0}] ASC\r\n", entity.Keys[key].Alias));
            sqlBuilder.AppendFormat(string.Join(", ", keys));

            sqlBuilder.Append(") WITH (IGNORE_DUP_KEY = ON));");
            return sqlBuilder.ToString();
        }
    }
}
