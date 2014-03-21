using System;
using System.Configuration;

namespace Transformalize.Configuration
{
    public class SqlOverride : ConfigurationElementCollection {
        private const string SQL = "sql";

        public EmptyConfigurationElement this[int index] {
            get { return BaseGet(index) as EmptyConfigurationElement; }
            set {
                if (BaseGet(index) != null) {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        [ConfigurationProperty(SQL, IsRequired = true)]
        public string Sql {
            get { return this[SQL] as string; }
            set { this[SQL] = value; }
        }

        public override bool IsReadOnly() {
            return false;
        }

        protected override ConfigurationElement CreateNewElement() {
            return new EmptyConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return Guid.NewGuid().ToString();
        }

    }
}