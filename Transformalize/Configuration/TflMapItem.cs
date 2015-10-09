using Cfg.Net;

namespace Transformalize.Configuration {
    public class TflMapItem : CfgNode {

        [Cfg(value = "", required = true, unique = true)]
        public string From { get; set; }

        [Cfg(value = "equals")]
        public string Operator { get; set; }

        [Cfg(value = "")]
        public string Parameter { get; set; }

        [Cfg(value = "")]
        public string To { get; set; }

    }
}