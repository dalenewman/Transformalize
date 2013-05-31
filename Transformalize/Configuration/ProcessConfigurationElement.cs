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

        [ConfigurationProperty("joins")]
        public JoinElementCollection Joins {
            get {
                return this["joins"] as JoinElementCollection;
            }
        }

    }
}