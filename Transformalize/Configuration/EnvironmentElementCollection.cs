using System.Configuration;

namespace Transformalize.Configuration {

    public class EnvironmentElementCollection : ConfigurationElementCollection {

        private const string DEFAULT = "default";

        public EnvironmentConfigurationElement this[int index] {
            get { return BaseGet(index) as EnvironmentConfigurationElement; }
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
            return new EnvironmentConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return ((EnvironmentConfigurationElement)element).Name.ToLower();
        }

        public EnvironmentConfigurationElement Get(string name) {
            foreach (EnvironmentConfigurationElement element in this) {
                if (element.Name.ToLower().Equals(name.ToLower())) {
                    return element;
                }
            }
            return null;
        }

        public void Add(EnvironmentConfigurationElement element) {
            BaseAdd(element);
        }

        [ConfigurationProperty(DEFAULT, IsRequired = false, DefaultValue = "")]
        public string Default {
            get { return this[DEFAULT] as string; }
            set { this[DEFAULT] = value; }
        }

    }
}