using System.Configuration;

namespace Transformalize.Configuration
{
    public class ProviderElementCollection : ConfigurationElementCollection
    {
        public ProviderConfigurationElement this[int index]
        {
            get { return BaseGet(index) as ProviderConfigurationElement; }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ProviderConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ProviderConfigurationElement) element).Name.ToLower();
        }
    }
}