using System;
using System.Configuration;

namespace Transformalize.Configuration
{
    public class SqlOverrideElementCollection : ConfigurationElementCollection {
        private const string SQL = "sql";
        private const string SCRIPT = "script";

        public EmptyConfigurationElement this[int index] {
            get { return BaseGet(index) as EmptyConfigurationElement; }
            set {
                if (BaseGet(index) != null) {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        [ConfigurationProperty(SQL, IsRequired = false, DefaultValue = "")]
        public string Sql {
            get { return this[SQL] as string; }
            set { this[SQL] = value; }
        }

        [ConfigurationProperty(SCRIPT, IsRequired = false, DefaultValue = "")]
        public string Script {
            get { return this[SCRIPT] as string; }
            set { this[SCRIPT] = value; }
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