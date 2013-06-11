using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RazorEngine.Templating;
using Transformalize.Configuration;

namespace Transformalize.Model {

    public class Process {
        private Dictionary<string, IField> _fields;

        public string Name;
        public Dictionary<string, Connection> Connections = new Dictionary<string, Connection>();
        public string Output;
        public string Time;
        public Connection OutputConnection;
        public Dictionary<string, Entity> Entities = new Dictionary<string, Entity>();
        public List<Join> Joins = new List<Join>();

        public Dictionary<string, IField> Fields {
            get {
                if (_fields == null) {
                    _fields = new Dictionary<string, IField>();
                    foreach (var entityKey in Entities.Keys) {
                        var entity = Entities[entityKey];
                        foreach (var fieldKey in entity.All.Keys) {
                            _fields[fieldKey] = entity.All[fieldKey];
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

        public string CreateOutputSql() {

            var sqlBuilder = new StringBuilder(Environment.NewLine);

            sqlBuilder.AppendFormat("CREATE TABLE [dbo].[{0}](\r\n", Output);
            sqlBuilder.Append(new FieldSqlWriter(Fields).ExpandXml().AddSystemFields().Output().Alias().DataType().AppendIf(" NOT NULL", FieldType.PrimaryKey).Write(",\r\n") + ",\r\n");
            sqlBuilder.AppendFormat("CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED (\r\n", Output);
            sqlBuilder.AppendLine(new FieldSqlWriter(Entities.Select(kv => kv.Value).First().Keys).Alias().Asc().Write());
            sqlBuilder.Append(") WITH (IGNORE_DUP_KEY = ON));");

            return sqlBuilder.ToString();
        }

        public string CreateOutputSqlWithTemplate() {
            using (var service = new TemplateService()) {
                var template = File.ReadAllText(@"Templates\CreateOutputTable.cshtml");
                return service.Parse(template, this, null, null);
            }
        }
    }
}
