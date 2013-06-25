using System.Configuration;

namespace Transformalize.Configuration {
    public class MapElementCollection : ConfigurationElementCollection {

        public MapConfigurationElement this[int index] {
            get {
                return BaseGet(index) as MapConfigurationElement;
            }
            set {
                if (BaseGet(index) != null) {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement() {
            return new MapConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return ((MapConfigurationElement)element).Name.ToLower();
        }

    }
}