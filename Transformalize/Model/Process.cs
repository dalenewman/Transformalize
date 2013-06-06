using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RazorEngine.Templating;
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

        public IEnumerable<string> OutputPrimaryKey() {
            return Fields.Where(f => f.FieldType == FieldType.Key && f.Entity.Equals(Entities.Keys.First())).Select(f => f.SqlWriter.Alias().Asc().Write());
        }

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
            sqlBuilder.Append(new FieldSqlWriter(OutputFields()).Alias().DataType().NotNull().Write(",\r\n") + ",\r\n");
            sqlBuilder.AppendFormat("CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED (\r\n", Output);
            sqlBuilder.AppendFormat(string.Join(", ", OutputPrimaryKey()));
            sqlBuilder.Append(") WITH (IGNORE_DUP_KEY = ON));");
          
            return sqlBuilder.ToString();
        }

        //public string CreateOutputSql() {
        //    using (var service = new TemplateService()) {
        //        var template = File.ReadAllText(@"Templates\CreateOutputTable.cshtml");
        //        return service.Parse(template, this, null, null);
        //    }
        //}
    }
}
