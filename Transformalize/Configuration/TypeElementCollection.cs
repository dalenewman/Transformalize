using System.Configuration;

namespace Transformalize.Configuration {
    public class TypeElementCollection : MyConfigurationElementCollection {

        private const string IGNORE_EMPTY = "ignore-empty";

        [ConfigurationProperty(IGNORE_EMPTY, IsRequired = false, DefaultValue = true)]
        public bool IgnoreEmpty {
            get { return (bool)this[IGNORE_EMPTY]; }
            set { this[IGNORE_EMPTY] = value; }
        }

        public TypeConfigurationElement this[int index] {
            get { return BaseGet(index) as TypeConfigurationElement; }
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
            return new TypeConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return ((TypeConfigurationElement)element).Type.ToLower();
        }

        public void Add(TypeConfigurationElement element) {
            BaseAdd(element);
        }
    }
}