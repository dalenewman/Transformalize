using Cfg.Net;

namespace Pipeline.Configuration {
    public class Schedule : CfgNode {

        [Cfg(required = true)]
        public string Name { get; set; }

        [Cfg(required = true)]
        public string Cron { get; set; }

        [Cfg(value = "default")]
        public string Mode { get; set; }
    }
}