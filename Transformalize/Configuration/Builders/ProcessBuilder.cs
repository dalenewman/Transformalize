namespace Transformalize.Configuration.Builders {

    public class ProcessBuilder {

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
    }
}