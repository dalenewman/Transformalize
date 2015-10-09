using Cfg.Net;
using Transformalize.Main;

namespace Transformalize.Configuration {
    public class TflIo : CfgNode {

        [Cfg(value = "", required = true)]
        public string Connection { get; set; }
        [Cfg(value = "")]
        public string Name { get; set; }
        [Cfg(value = "")]
        public string RunField { get; set; }
        [Cfg(value = "Equal", domain = Common.ValidComparisons)]
        public string RunOperator { get; set; }
        [Cfg(value = Common.DefaultValue)]
        public string RunType { get; set; }
        [Cfg(value = "")]
        public string RunValue { get; set; }

    }
}