using System.Configuration;

namespace Transformalize.Configuration {
    public class OutputElementCollection : ConfigurationElementCollection {

        public EmptyConfigurationElement this[int index] {
            get {
                return BaseGet(index) as EmptyConfigurationElement;
            }
            set {
                if (BaseGet(index) != null) {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement() {
            return new EmptyConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return System.Guid.NewGuid();
        }

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

        [ConfigurationProperty("connection", IsRequired = true)]
        public string Connection {
            get {
                return this["connection"] as string;
            }
            set { this["connection"] = value; }
        }

    }
}