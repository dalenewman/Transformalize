using System.Configuration;

namespace Transformalize.Configuration
{
    public class JoinConfigurationElement : ConfigurationElement {

        [ConfigurationProperty("leftEntity", IsRequired = true)]
        public string LeftEntity {
            get {
                return this["leftEntity"] as string;
            }
            set { this["leftEntity"] = value; }
        }

        [ConfigurationProperty("leftField", IsRequired = true)]
        public string LeftField {
            get {
                return this["leftField"] as string;
            }
            set { this["leftField"] = value; }
        }

        [ConfigurationProperty("rightEntity", IsRequired = true)]
        public string RightEntity {
            get {
                return this["rightEntity"] as string;
            }
            set { this["rightEntity"] = value; }
        }

        [ConfigurationProperty("rightField", IsRequired = true)]
        public string RightField {
            get {
                return this["rightField"] as string;
            }
            set { this["rightField"] = value; }
        }

    }
}