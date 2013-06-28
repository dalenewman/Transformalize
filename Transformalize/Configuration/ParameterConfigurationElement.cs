using System.Configuration;

namespace Transformalize.Configuration
{
    public class ParameterConfigurationElement : ConfigurationElement {

        [ConfigurationProperty("entity", IsRequired = true)]
        public string Entity {
            get {
                return this["entity"] as string;
            }
            set { this["entity"] = value; }
        }

        [ConfigurationProperty("field", IsRequired = true)]
        public string Field {
            get {
                return this["field"] as string;
            }
            set { this["field"] = value; }
        }

    }
}