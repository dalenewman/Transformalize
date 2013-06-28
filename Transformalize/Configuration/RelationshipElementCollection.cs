using System.Configuration;

namespace Transformalize.Configuration
{
    public class RelationshipElementCollection : ConfigurationElementCollection {

        public RelationshipConfigurationElement this[int index] {
            get {
                return BaseGet(index) as RelationshipConfigurationElement;
            }
            set {
                if (BaseGet(index) != null) {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement() {
            return new RelationshipConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            var join = element as RelationshipConfigurationElement;

            return string.Concat(join.LeftEntity, join.LeftField, join.RightEntity, join.RightField);
        }
    }
}