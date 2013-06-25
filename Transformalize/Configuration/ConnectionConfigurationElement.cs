using System.Configuration;

namespace Transformalize.Configuration
{
    public class ConnectionConfigurationElement : ConfigurationElement {

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name {
            get {
                return this["name"] as string;
            }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("value", IsRequired = true)]
        public string Value {
            get {
                return this["value"] as string;
            }
            set { this["value"] = value; }
        }

        [ConfigurationProperty("year", IsRequired = false, DefaultValue = 2008)]
        public int Year {
            get {
                return (int) this["year"];
            }
            set { this["year"] = value; }
        }

        [ConfigurationProperty("provider", IsRequired = false, DefaultValue = "")]
        public string Provider {
            get {
                return this["provider"] as string;
            }
            set { this["provider"] = value; }
        }

        [ConfigurationProperty("outputBatchSize", IsRequired = false, DefaultValue = 1000)]
        public int OutputBatchSize {
            get {
                return (int)this["outputBatchSize"];
            }
            set { this["outputBatchSize"] = value; }
        }

        [ConfigurationProperty("inputBatchSize", IsRequired = false, DefaultValue = 500)]
        public int InputBatchSize {
            get {
                return (int)this["inputBatchSize"];
            }
            set { this["inputBatchSize"] = value; }
        }

    }
}