using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflResponse : CfgNode {
        [Cfg(value = "OK")]
        public string Status { get; set; }
        [Cfg(value = "")]
        public string Message { get; set; }
        [Cfg(value = (long)0)]
        public long Time { get; set; }
        [Cfg()]
        public List<TflRow> Rows { get; set; }
    }
}