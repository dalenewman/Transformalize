using System.Configuration;

namespace Transformalize.Configuration
{
    public class EntityElementCollection : ConfigurationElementCollection {

        public EntityConfigurationElement this[int index] {
            get {
                return BaseGet(index) as EntityConfigurationElement;
            }
            set {
                if (BaseGet(index) != null) {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement() {
            return new EntityConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return ((EntityConfigurationElement)element).Name.ToLower();
        }
        
    }
}