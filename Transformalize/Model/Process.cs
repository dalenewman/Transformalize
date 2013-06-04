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

        private IList<IField> _fields;

        public string Name;
        public Dictionary<string, Connection> Connections = new Dictionary<string, Connection>();
        public string Output;
        public string Time;
        public Connection OutputConnection;
        public Dictionary<string, Entity> Entities = new Dictionary<string, Entity>();
        public List<Join> Joins = new List<Join>();

        public IList<IField> Fields {
            get {
                if (_fields == null) {
                    _fields = new List<IField>();
                    foreach (var entity in Entities.Select(kv => kv.Value)) {
                        foreach (var field in entity.All.Select(kv => kv.Value)) {
                            if (field.InnerXml.Any()) {
                                foreach (var xmlField in field.InnerXml.Select(kv => kv.Value)) {
                                    _fields.Add(xmlField);
                                }
                            }
                            else {
                                _fields.Add(field);
                            }
                        }
                    }
                }
                return _fields;
            }
        }

        public string TruncateOutputSql() {
            return SqlTemplates.TruncateTable(Output);
        }

        public string DropOutputSql() {
            return SqlTemplates.DropTable(Output);
        }

        public IEnumerable<IField> OutputFields() {
            var fields = new List<IField> {
                new Field() {Alias = ROW_VERSION_KEY, Type = "System.RowVersion"},
                new Field() {Alias = TIME_KEY_KEY, Type = "System.Int32"},
                new Field() {Alias = LOAD_DATE_KEY, Type = "System.DateTime"}
            };
            fields.AddRange(Fields.Where(f => f.Output));
            return fields.OrderBy(f => f.Alias);
        }

        public string CreateOutputSql() {

            var sqlBuilder = new StringBuilder(Environment.NewLine);
            sqlBuilder.AppendFormat("CREATE TABLE [dbo].[{0}](\r\n", Output);

            foreach (var field in OutputFields()) {
                sqlBuilder.AppendLine(field.AsDefinition() + ",");
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
