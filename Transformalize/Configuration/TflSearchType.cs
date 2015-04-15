using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {

    public class TflDataSet : CfgNode {
        [Cfg(required = true)]
        public string Name { get; set; }

        [Cfg()]
        public List<Dictionary<string,string>> Rows { get; set; }
    }

    public class TflSearchType : CfgNode {
        [Cfg(value = "", required = true, unique = true)]
        public string Name { get; set; }
        [Cfg(value = true)]
        public bool Store { get; set; }
        [Cfg(value = true)]
        public bool Index { get; set; }
        [Cfg(value = false)]
        public bool MultiValued { get; set; }
        [Cfg(value = "")]
        public string Analyzer { get; set; }
        [Cfg(value = true)]
        public bool Norms { get; set; }
    }
}