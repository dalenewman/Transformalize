using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;
using Transformalize.Libs.Cfg.Net.Loggers;
using Transformalize.Libs.Cfg.Net.Parsers;
using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Configuration {
    public class TflRoot : CfgNode {

        [Cfg(sharedProperty = "default", sharedValue = "")]
        public List<TflEnvironment> Environments { get; set; }
        [Cfg(required = true)]
        public List<TflProcess> Processes { get; set; }
        [Cfg()]
        public List<TflResponse> Response { get; set; }

        public TflRoot(
                string xml, 
                Dictionary<string, string> parameters = null,
                ILogger logger = null)
            : base(new XDocumentParser(), logger) {

            Load(xml, parameters) ;
        }

        // Custom constructor takeing an already created TflProcess
        public TflRoot(TflProcess process) {
            var root = new TflRoot { Processes = new List<TflProcess> { process } };
            var json = JsonConvert.SerializeObject(root, new JsonSerializerSettings() {NullValueHandling = NullValueHandling.Ignore});
            Load(json);
        }

        public TflRoot() {
        }
    }
}