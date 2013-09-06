using System.Configuration;

namespace Transformalize.Configuration
{
    public class FieldSearchTypeElementCollection : ConfigurationElementCollection
    {

        public override bool IsReadOnly()
        {
            return false;
        }

        public FieldSearchTypeConfigurationElement this[int index]
        {
            get
            {
                return BaseGet(index) as FieldSearchTypeConfigurationElement;
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new FieldSearchTypeConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FieldSearchTypeConfigurationElement)element).Type.ToLower();
        }

    }
}