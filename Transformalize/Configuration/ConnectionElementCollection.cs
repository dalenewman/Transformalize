using System.Configuration;

namespace Transformalize.Configuration {
    public class ConnectionElementCollection : ConfigurationElementCollection {

        public ConnectionConfigurationElement this[int index] {
            get {
                return BaseGet(index) as ConnectionConfigurationElement;
            }
            set {
                if (BaseGet(index) != null) {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement() {
            return new ConnectionConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return ((ConnectionConfigurationElement)element).Name.ToLower();
        }

    }
}