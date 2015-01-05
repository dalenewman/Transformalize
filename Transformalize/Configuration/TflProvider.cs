using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflProvider : CfgNode {
        public TflProvider() {
            Property("name", string.Empty, true, true);
            Property("type", string.Empty, true, false);
        }
    }
}