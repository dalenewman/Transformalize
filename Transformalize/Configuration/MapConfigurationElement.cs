using System.Configuration;

namespace Transformalize.Configuration
{
    public class MapConfigurationElement : ConfigurationElement {

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name {
            get {
                return this["name"] as string;
            }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("items")]
        public ItemElementCollection Items {
            get {
                return this["items"] as ItemElementCollection;
            }
        }

    }
}