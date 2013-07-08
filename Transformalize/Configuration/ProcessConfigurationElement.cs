using System.Configuration;

namespace Transformalize.Configuration {

    public class ProcessConfigurationElement : ConfigurationElement {

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name {
            get {
                return this["name"] as string;
            }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("output", IsRequired = true)]
        public string Output {
            get {
                return this["output"] as string;
            }
            set { this["output"] = value; }
        }

        [ConfigurationProperty("connection", IsRequired = true)]
        public string Connection {
            get {
                return this["connection"] as string;
            }
            set { this["connection"] = value; }
        }

        [ConfigurationProperty("connections")]
        public ConnectionElementCollection Connections {
            get {
                return this["connections"] as ConnectionElementCollection;
            }
        }

        [ConfigurationProperty("maps")]
        public MapElementCollection Maps {
            get {
                return this["maps"] as MapElementCollection;
            }
        }

        [ConfigurationProperty("entities")]
        public EntityElementCollection Entities {
            get {
                return this["entities"] as EntityElementCollection;
            }
        }

        [ConfigurationProperty("relationships")]
        public RelationshipElementCollection Relationships {
            get {
                return this["relationships"] as RelationshipElementCollection;
            }
        }

        [ConfigurationProperty("transforms")]
        public TransformElementCollection Transforms {
            get {
                return this["transforms"] as TransformElementCollection;
            }
        }

    }
}