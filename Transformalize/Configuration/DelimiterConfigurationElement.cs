using System.Configuration;

namespace Transformalize.Configuration {
    public class DelimiterConfigurationElement : ConfigurationElement {

        private const string NAME = "name";
        private const string CHARACTER = "character";

        [ConfigurationProperty(NAME, IsRequired = true)]
        public string Name {
            get { return this[NAME] as string; }
            set { this[NAME] = value; }
        }

        [ConfigurationProperty(CHARACTER, IsRequired = true)]
        public string Character {
            get { return this[CHARACTER] as string; }
            set { this[CHARACTER] = value; }
        }
    }
}