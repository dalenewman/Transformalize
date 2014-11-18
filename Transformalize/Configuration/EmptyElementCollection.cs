using System;
using System.Configuration;

namespace Transformalize.Configuration {
    public class EmptyElementCollection : ConfigurationElementCollection {

        public EmptyConfigurationElement this[int index] {
            get { return BaseGet(index) as EmptyConfigurationElement; }
            set {
                if (BaseGet(index) != null) {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public override bool IsReadOnly() {
            return false;
        }

        protected override ConfigurationElement CreateNewElement() {
            return new EmptyConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return Guid.NewGuid();
        }

    }
}