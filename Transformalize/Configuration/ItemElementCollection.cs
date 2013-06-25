using System.Configuration;

namespace Transformalize.Configuration {
    public class ItemElementCollection : ConfigurationElementCollection {

        public ItemConfigurationElement this[int index] {
            get {
                return BaseGet(index) as ItemConfigurationElement;
            }
            set {
                if (BaseGet(index) != null) {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement() {
            return new ItemConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return ((ItemConfigurationElement)element).From.ToLower();
        }

    }
}