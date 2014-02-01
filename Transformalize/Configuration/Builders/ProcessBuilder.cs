namespace Transformalize.Configuration.Builders {

    public class ProcessBuilder : IFieldHolder {

        private readonly ProcessConfigurationElement _process;

        public ProcessBuilder(string name) {
            _process = new ProcessConfigurationElement() { Name = name, Star = name + "Star" };

            _process.Providers.Add(
                new ProviderConfigurationElement {
                    Name = "sqlserver",
                    Type = "System.Data.SqlClient.SqlConnection, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
                },
                new ProviderConfigurationElement {
                    Name = "mysql",
                    Type = "MySql.Data.MySqlClient.MySqlConnection, MySql.Data"
                },
                new ProviderConfigurationElement {
                    Name = "file",
                    Type = string.Empty
                },
                new ProviderConfigurationElement {
                    Name = "folder",
                    Type = string.Empty
                },
                new ProviderConfigurationElement {
                    Name = "internal",
                    Type = string.Empty
                }
            );

            _process.SearchTypes.Add(
                new SearchTypeConfigurationElement {
                    Name = "none",
                    MultiValued = false,
                    Store = false,
                    Index = false
                },
                new SearchTypeConfigurationElement {
                    Name = "default",
                    MultiValued = false,
                    Store = true,
                    Index = true
                }
            );

        }

        public ProcessConfigurationElement Process() {
            return _process;
        }

        public ConnectionBuilder Connection(string name) {
            var connection = new ConnectionConfigurationElement() { Name = name };
            _process.Connections.Add(connection);
            return new ConnectionBuilder(this, connection);
        }

        public MapBuilder Map(string name, string sql = "") {
            var map = new MapConfigurationElement { Name = name };
            map.Items.Sql = sql;
            _process.Maps.Add(map);

            return new MapBuilder(this, map);
        }

        public EntityBuilder Entity(string name) {
            var entity = new EntityConfigurationElement() { Name = name };
            _process.Entities.Add(entity);
            return new EntityBuilder(this, entity);
        }

        public RelationshipBuilder Relationship() {
            var relationship = new RelationshipConfigurationElement();
            _process.Relationships.Add(relationship);
            return new RelationshipBuilder(this, relationship);
        }

        public TemplateBuilder Template(string name) {
            var template = new TemplateConfigurationElement { Name = name };
            _process.Templates.Add(template);
            return new TemplateBuilder(this, template);
        }

        public SearchTypeBuilder SearchType(string name) {
            var searchType = new SearchTypeConfigurationElement() { Name = name };
            _process.SearchTypes.Add(searchType);
            return new SearchTypeBuilder(this, searchType);
        }

        public FieldBuilder CalculatedField(string name) {
            var calculatedField = new FieldConfigurationElement() { Name = name };
            _process.CalculatedFields.Add(calculatedField);
            return new FieldBuilder(this, calculatedField);
        }

        public FieldBuilder Field(string name) {
            var calculatedField = new FieldConfigurationElement() { Name = name };
            _process.CalculatedFields.Add(calculatedField);
            return new FieldBuilder(this, calculatedField);
        }

        public ProcessBuilder TemplatePath(string path) {
            _process.Templates.Path = path;
            return this;
        }

        public ProcessBuilder ScriptPath(string path) {
            _process.Scripts.Path = path;
            return this;
        }

        public ScriptBuilder Script(string name) {
            var script = new ScriptConfigurationElement() { Name = name };
            _process.Scripts.Add(script);
            return new ScriptBuilder(this, script);
        }

        public ProcessBuilder Star(string star) {
            _process.Star = star;
            return this;
        }

        public ProcessBuilder TimeZone(string timeZone) {
            _process.TimeZone = timeZone;
            return this;
        }

    }
}