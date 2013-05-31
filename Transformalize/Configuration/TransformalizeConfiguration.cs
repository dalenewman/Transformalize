using System.Configuration;

namespace Transformalize.Configuration {

    public class TransformalizeConfiguration : ConfigurationSection {

        [ConfigurationProperty("processes")]
        public ProcessElementCollection Processes {
            get {
                return this["processes"] as ProcessElementCollection;
            }
        }

    }
}
