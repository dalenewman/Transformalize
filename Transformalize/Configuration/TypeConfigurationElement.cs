using System.Configuration;

namespace Transformalize.Configuration {
    public class TypeConfigurationElement : ConfigurationElement {

        private const string TYPE = "type";

        [ConfigurationProperty(TYPE, IsRequired = true)]
        public string Type {
            get { return this[TYPE] as string; }
            set { this[TYPE] = value; }
        }
    }
}