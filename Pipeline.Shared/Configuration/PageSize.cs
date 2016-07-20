using Cfg.Net;

namespace Pipeline.Configuration {
    public class PageSize : CfgNode {
        [Cfg]
        public int Size { get; set; }
    }
}