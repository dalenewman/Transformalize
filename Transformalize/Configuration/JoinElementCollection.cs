using System.Configuration;

namespace Transformalize.Configuration {
    public class JoinElementCollection : ConfigurationElementCollection {

        public JoinConfigurationElement this[int index] {
            get {
                return BaseGet(index) as JoinConfigurationElement;
            }
            set {
                if (BaseGet(index) != null) {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement() {
            return new JoinConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            var join = (JoinConfigurationElement)element;
            return string.Concat(join.LeftField, join.RightField).ToLower();
        }

    }
}