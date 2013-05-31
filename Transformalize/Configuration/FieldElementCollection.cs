using System.Configuration;

namespace Transformalize.Configuration
{
    public class FieldElementCollection : ConfigurationElementCollection {

        public FieldConfigurationElement this[int index] {
            get {
                return BaseGet(index) as FieldConfigurationElement;
            }
            set {
                if (BaseGet(index) != null) {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement() {
            return new FieldConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return ((FieldConfigurationElement)element).Alias.ToLower();
        }

    }
}