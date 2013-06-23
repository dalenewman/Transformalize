using System;
using System.Configuration;

namespace Transformalize.Configuration {
    public class TransformElementCollection : ConfigurationElementCollection {

        public TransformConfigurationElement this[int index] {
            get {
                return BaseGet(index) as TransformConfigurationElement;
            }
            set {
                if (BaseGet(index) != null) {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement() {
            return new TransformConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return Guid.NewGuid();
        }
    }
}