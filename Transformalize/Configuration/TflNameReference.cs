using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflNameReference : CfgNode {
        [Cfg(required = true, unique = true)]
        public string Name { get; set; }
    }
}