using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {

    public class TflMap : CfgNode {

        [Cfg(value = "input")]
        public string Connection { get; set; }
        [Cfg(value = "", required = true, unique = true)]
        public string Name { get; set; }
        [Cfg(value = "")]
        public string Query { get; set; }

        [Cfg(required = false)]
        public List<TflMapItem> Items { get; set; }

    }
}