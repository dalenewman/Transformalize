using System.Configuration;

namespace Transformalize.Configuration {
    public class XmlElementCollection : ConfigurationElementCollection {

        public XmlConfigurationElement this[int index] {
            get {
                return BaseGet(index) as XmlConfigurationElement;
            }
            set {
                if (BaseGet(index) != null) {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement() {
            return new XmlConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return ((XmlConfigurationElement)element).Alias.ToLower();
        }

        [ConfigurationProperty("xPath", IsRequired = true)]
        public string XPath {
            get {
                return this["xPath"] as string;
            }
            set { this["xPath"] = value; }
        }
    }
}