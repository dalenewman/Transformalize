using System.Configuration;

namespace Transformalize.Configuration
{
    public class OutputConfigurationElement : ConfigurationElement {

        private const string NAME = "name";
        private const string CONNECTION = "connection";

        [ConfigurationProperty(NAME, IsRequired = true)]
        public string Name {
            get { return this[NAME] as string; }
            set { this[NAME] = value; }
        }

        [ConfigurationProperty(CONNECTION, IsRequired = false)]
        public string Connection {
            get { return this[CONNECTION] as string; }
            set { this[CONNECTION] = value; }
        }

        public override bool IsReadOnly() {
            return false;
        }
    }
}