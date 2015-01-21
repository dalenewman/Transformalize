using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflJoin : CfgNode {

        [Cfg( /* name= "left-field" */ value = "", required = true)]
        public string LeftField { get; set; }
        [Cfg( /* name= "right-field" */ value = "", required = true)]
        public string RightField { get; set; }

    }
}