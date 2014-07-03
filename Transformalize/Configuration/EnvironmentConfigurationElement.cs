using System.Configuration;

namespace Transformalize.Configuration {

    public class EnvironmentConfigurationElement : ConfigurationElement {

        private const string PARAMETERS = "parameters";
        private const string NAME = "name";

        [ConfigurationProperty(PARAMETERS)]
        public ParameterElementCollection Parameters {
            get { return this[PARAMETERS] as ParameterElementCollection; }
        }

        [ConfigurationProperty(NAME, IsRequired = true)]
        public string Name {
            get { return this[NAME] as string; }
            set { this[NAME] = value; }
        }

    }
}