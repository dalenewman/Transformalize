using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflFilter : CfgNode {

        [Cfg( /* name= "continuation" */ value = "AND")]
        public string Continuation { get; set; }
        [Cfg( /* name= "expression" */ value = "")]
        public string Expression { get; set; }
        [Cfg( /* name= "left" */ value = "")]
        public string Left { get; set; }
        [Cfg( /* name= "operator" */ value = "Equal")]
        public string Operator { get; set; }
        [Cfg( /* name= "right" */ value = "")]
        public string Value { get; set; }

    }
}