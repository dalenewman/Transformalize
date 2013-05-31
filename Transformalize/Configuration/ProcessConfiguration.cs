using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Transformalize.Configuration {

    public class ProcessConfiguration {

        const string ROW_VERSION_KEY = "RowVersion";
        const string TIME_KEY_KEY = "TimeKey";
        const string LOAD_DATE_KEY = "LoadDate";

        private Dictionary<string, IField> _fields;

        public string Name;
        public Dictionary<string, Connection> Connections = new Dictionary<string, Connection>();
        public string Output;
        public string Time;
        public Connection Connection;
        public Dictionary<string, Entity> Entities = new Dictionary<string, Entity>();
        public List<Join> Joins = new List<Join>();

        public ProcessConfiguration() { }
        public ProcessConfiguration(ProcessConfigurationElement process) {
            InitializeFromProcess(process);
        }

        private void InitializeFromProcess(ProcessConfigurationElement process) {
            Name = process.Name;

            foreach (ConnectionConfigurationElement element in process.Connections) {
                Connections.Add(element.Name, new Connection { Value = element.Value, Provider = element.Provider });
            }

            foreach (EntityConfigurationElement entityElement in process.Entities) {
                var entity = new Entity { Schema = entityElement.Schema, Name = entityElement.Name, Connection = Connections[entityElement.Connection] };

                foreach (FieldConfigurationElement fieldElement in entityElement.Keys) {
                    var key = new Field {
                        Entity = entity.Name,
                        Schema = entity.Schema,
                        Name = fieldElement.Name,
                        Type = fieldElement.Type,
                        Alias = fieldElement.Alias,
                        Length = fieldElement.Length,
                        Precision = fieldElement.Precision,
                        Scale = fieldElement.Scale,
                        Output = fieldElement.Output,
                        FieldType = FieldType.Key
                    };
                    entity.Keys.Add(fieldElement.Alias, key);
                    entity.All.Add(fieldElement.Alias, key);

                    if (entityElement.Version.Equals(fieldElement.Name)) {
                        entity.Version = key;
                    }
                }

                foreach (FieldConfigurationElement fieldElement in entityElement.Fields) {
                    var field = new Field {
                        Entity = entity.Name,
                        Schema = entity.Schema,
                        Name = fieldElement.Name,
                        Type = fieldElement.Type,
                        Alias = fieldElement.Alias,
                        Length = fieldElement.Length,
                        Precision = fieldElement.Precision,
                        Scale = fieldElement.Scale,
                        Output = fieldElement.Output,
                        FieldType = FieldType.Field
                    };
                    foreach (XmlConfigurationElement xmlElement in fieldElement.Xml) {
                        field.Xml.Add(xmlElement.Alias, new Xml {
                            Entity = entity.Name,
                            Schema = entity.Schema,
                            Parent = fieldElement.Name,
                            XPath = fieldElement.Xml.XPath + xmlElement.XPath,
                            Name = xmlElement.XPath,
                            Alias = xmlElement.Alias,
                            Index = xmlElement.Index,
                            Type = xmlElement.Type,
                            Length = xmlElement.Length,
                            Precision = xmlElement.Precision,
                            Scale = xmlElement.Scale,
                            Output = xmlElement.Output
                        });
                    }

                    entity.Fields.Add(fieldElement.Alias, field);
                    entity.All.Add(fieldElement.Alias, field);

                    if (entityElement.Version.Equals(fieldElement.Name)) {
                        entity.Version = field;
                    }
                }

                Entities.Add(entityElement.Name, entity);
            }

            foreach (JoinConfigurationElement joinElement in process.Joins) {
                var join = new Join();
                join.LeftEntity = Entities[joinElement.LeftEntity];
                join.LeftField = join.LeftEntity.All[joinElement.LeftField];
                join.RightEntity = Entities[joinElement.RightEntity];
                join.RightField = join.RightEntity.All[joinElement.RightField];
                Joins.Add(join);
            }

            Output = process.Output;
            Time = process.Time;
            Connection = Connections["output"];
        }

        private static string TruncateSql(string name) {
            return string.Format(@"
                IF EXISTS(
        	        SELECT *
        	        FROM INFORMATION_SCHEMA.TABLES
        	        WHERE TABLE_SCHEMA = 'dbo'
        	        AND TABLE_NAME = '{0}'
                )	TRUNCATE TABLE [{0}];
            ", name);
        }

        private static string DropSql(string name) {
            return string.Format(@"
                IF EXISTS(
        	        SELECT *
        	        FROM INFORMATION_SCHEMA.TABLES
        	        WHERE TABLE_SCHEMA = 'dbo'
        	        AND TABLE_NAME = '{0}'
                )	DROP TABLE [{0}];
            ", name);
        }

        public string TruncateOutputSql() {
            return TruncateSql(Output);
        }

        public string DropOutputSql() {
            return DropSql(Output);
        }

        public string CreateEntityTrackerSql() {
            return @"
                CREATE TABLE EntityTracker(
	                EntityTrackerKey INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	                EntityName NVARCHAR(100) NOT NULL,
	                VersionByteArray BINARY(8) NULL,
	                VersionDateTime DATETIME NULL,
                    VersionInt64 BIGINT NULL,
                    VersionInt32 INT NULL,
	                LastProcessedDate DATETIME NOT NULL
                );
            ";
        }

        public string TruncateEntityTrackerSql() {
            return TruncateSql("EntityTracker");
        }

        public string DropEntityTrackerSql() {
            return DropSql("EntityTracker");
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

        public string CreateSql() {

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
