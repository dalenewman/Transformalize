using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflEnvironment : CfgNode {
        [Cfg(required = true, unique = true)]
        public string Name { get; set; }
        [Cfg(required = true)]
        public List<TflParameter> Parameters { get; set; }
        [Cfg()]
        public string Default { get; set; }
    }
}