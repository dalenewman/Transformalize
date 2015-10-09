using System.Collections.Generic;
using Cfg.Net;

namespace Transformalize.Configuration {
    public class TflRelationship : CfgNode {

        [Cfg(value = "", required = true, unique = false)]
        public string LeftEntity { get; set; }
        [Cfg(value = "", required = true, unique = false)]
        public string RightEntity { get; set; }
        [Cfg(value = "")]
        public string LeftField { get; set; }
        [Cfg(value = "")]
        public string RightField { get; set; }
        [Cfg(value = false)]
        public bool Index { get; set; }

        [Cfg()]
        public List<TflJoin> Join { get; set; }
    }
}