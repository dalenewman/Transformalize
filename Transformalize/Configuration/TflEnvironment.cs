using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflEnvironment : CfgNode {

        public TflEnvironment() {
            Property("name", string.Empty, true, true);
            Collection<TflParameter>("parameters", true);
        }

        public string Name { get; set; }
        public string Default { get; set; }
        public List<TflParameter> Parameters { get; set; }
    }
}