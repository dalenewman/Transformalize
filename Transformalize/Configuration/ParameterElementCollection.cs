using System.Configuration;

namespace Transformalize.Configuration {
    public class ParameterElementCollection : ConfigurationElementCollection {

        public ParameterConfigurationElement this[int index] {
            get {
                return BaseGet(index) as ParameterConfigurationElement;
            }
            set {
                if (BaseGet(index) != null) {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement() {
            return new ParameterConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            var parameter = (ParameterConfigurationElement)element;
            return string.Concat(parameter.Entity, parameter.Field).ToLower();
        }

    }
}