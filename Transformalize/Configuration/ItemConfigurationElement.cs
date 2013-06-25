using System.Configuration;

namespace Transformalize.Configuration
{
    public class ItemConfigurationElement : ConfigurationElement {

        [ConfigurationProperty("from", IsRequired = true)]
        public string From {
            get {
                return this["from"] as string;
            }
            set { this["from"] = value; }
        }

        [ConfigurationProperty("to", IsRequired = true)]
        public string To {
            get {
                return this["to"] as string;
            }
            set { this["to"] = value; }
        }

        [ConfigurationProperty("operator", IsRequired = false, DefaultValue = "equals")]
        public string Operator {
            get {
                return this["operator"] as string;
            }
            set { this["operator"] = value; }
        }

    }
}