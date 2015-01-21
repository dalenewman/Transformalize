using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflProvider : CfgNode {
        [Cfg(required = true, unique = true)]
        public string Name { get; set; }
        [Cfg(required = true)]
        public string Type { get; set; }
    }
}