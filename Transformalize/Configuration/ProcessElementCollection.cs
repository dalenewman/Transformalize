using System;
using System.Configuration;

namespace Transformalize.Configuration {
    public class ProcessElementCollection : ConfigurationElementCollection {

        public ProcessConfigurationElement this[int index] {
            get {
                return BaseGet(index) as ProcessConfigurationElement;
            }
            set {
                if (BaseGet(index) != null) {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement() {
            return new ProcessConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return ((ProcessConfigurationElement)element).Name.ToLower();
        }

        public ProcessConfigurationElement Get(string name) {
            foreach (ProcessConfigurationElement element in this) {
                if (element.Name.ToLower().Equals(name.ToLower())) {
                    return element;
                }
            }
            throw new ArgumentException(string.Format("There are no processes named {0} defined!", name));
        }
    }
}