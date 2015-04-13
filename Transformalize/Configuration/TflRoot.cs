using System.Collections.Generic;
using System.Web.WebSockets;
using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflRoot : CfgNode {

        [Cfg(sharedProperty = "default", sharedValue = "")]
        public List<TflEnvironment> Environments { get; set; }
        [Cfg(required = true)]
        public List<TflProcess> Processes { get; set; }
        [Cfg()]
        public List<TflResponse> Response { get; set; }

        // Normal Constructor taking an XML or JSON confuration.
        public TflRoot(string cfg, Dictionary<string, string> parameters = null) {
            Load(cfg, parameters);
        }

        // Custom constructor takeing an already created TflProcess
        public TflRoot(TflProcess process) {
            var root = new TflRoot { Processes = new List<TflProcess> { process } };
            var json = Libs.Newtonsoft.Json.JsonConvert.SerializeObject(root);
            Load(json);
        }

        public TflRoot() {
        }
    }
}