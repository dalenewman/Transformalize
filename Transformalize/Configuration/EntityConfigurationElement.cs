using System.Configuration;

namespace Transformalize.Configuration
{
    public class EntityConfigurationElement : ConfigurationElement {

        [ConfigurationProperty("schema", IsRequired = false, DefaultValue = "dbo")]
        public string Schema {
            get {
                return this["schema"] as string;
            }
            set { this["schema"] = value; }
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name {
            get {
                return this["name"] as string;
            }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("connection", IsRequired = false, DefaultValue = "")]
        public string Connection {
            get {
                return this["connection"] as string;
            }
            set { this["connection"] = value; }
        }

        [ConfigurationProperty("primaryKey")]
        public FieldElementCollection PrimaryKey {
            get {
                return this["primaryKey"] as FieldElementCollection;
            }
        }

        [ConfigurationProperty("fields")]
        public FieldElementCollection Fields {
            get {
                return this["fields"] as FieldElementCollection;
            }
        }

        [ConfigurationProperty("version", IsRequired = true)]
        public string Version {
            get {
                return this["version"] as string;
            }
            set { this["version"] = value; }
        }
    }
}