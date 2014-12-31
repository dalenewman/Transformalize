using System.Collections.Generic;

namespace Transformalize.Configuration {
    public class TflEnvironment : CfgNode {

        public TflEnvironment() {
            Property("name", string.Empty, true, true);
            Class<TflParameter>("parameters", true);
        }

        public string Name { get; set; }
        public List<TflParameter> Parameters { get; set; }
    }
}