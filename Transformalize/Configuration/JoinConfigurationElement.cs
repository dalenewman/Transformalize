using System.Configuration;

namespace Transformalize.Configuration
{
    public class JoinConfigurationElement : ConfigurationElement {

        [ConfigurationProperty("leftField", IsRequired = true)]
        public string LeftField {
            get {
                return this["leftField"] as string;
            }
            set { this["leftField"] = value; }
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