using System.Configuration;

namespace Transformalize.Configuration {

    public class OutputElementCollection : ConfigurationElementCollection {

        public OutputConfigurationElement this[int index] {
            get { return BaseGet(index) as OutputConfigurationElement; }
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
            return new OutputConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return ((OutputConfigurationElement)element).Name;
        }

        public void Add(OutputConfigurationElement output) {
            BaseAdd(output);
        }
    }
}