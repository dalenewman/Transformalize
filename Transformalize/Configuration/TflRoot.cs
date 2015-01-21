using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflRoot : CfgNode {

        [Cfg(sharedProperty = "default", sharedValue = "")]
        public List<TflEnvironment> Environments { get; set; }
        [Cfg(required = true)]
        public List<TflProcess> Processes { get; set; }
        [Cfg()]
        public List<TflResponse> Response { get; set; }

        public TflRoot(string xml, Dictionary<string, string> parameters) {
            Load(xml, parameters);
        }
    }
}