using System.Configuration;

namespace Transformalize.Configuration {
    public class DelimiterElementCollection : MyConfigurationElementCollection {

        public DelimiterConfigurationElement this[int index] {
            get { return BaseGet(index) as DelimiterConfigurationElement; }
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
            return new DelimiterConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return ((DelimiterConfigurationElement)element).Character.ToLower();
        }

        public void Add(DelimiterConfigurationElement element) {
            BaseAdd(element);
        }
    }
}