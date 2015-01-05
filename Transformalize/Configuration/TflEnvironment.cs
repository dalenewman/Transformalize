using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflEnvironment : CfgNode {
        public TflEnvironment() {
            Property("name", string.Empty, true, true);
            Collection<TflParameter>("parameters", true);
        }
    }
}