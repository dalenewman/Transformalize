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

        [ConfigurationProperty("time", IsRequired = true)]
        public string Time {
            get {
                return this["time"] as string;
            }
            set { this["time"] = value; }
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