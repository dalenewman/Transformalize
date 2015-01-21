using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {

    public class TflTemplate : CfgNode {

        [Cfg( required = true, unique = true)]
        public string Name { get; set; }
        [Cfg( value = "raw", domain = "raw,html")]
        public string ContentType { get; set; }
        [Cfg( required = true, unique = true)]
        public string File { get; set; }
        [Cfg( value = false)]
        public bool Cache { get; set; }
        [Cfg( value = true)]
        public bool Enabled { get; set; }
        [Cfg( value = "razor", domain = "razor,velocity")]
        public string Engine { get; set; }  

        [Cfg()]
        public List<TflParameter> Parameters { get; set; }
        [Cfg()]
        public List<TflAction> Actions { get; set; }

    }
}