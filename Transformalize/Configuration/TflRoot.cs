using System.Collections.Generic;
using Cfg.Net;
using Cfg.Net.Contracts;
using Cfg.Net.Reader;
using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Configuration {
    public class TflRoot : CfgNode {

        [Cfg()]
        public string Environment { get; set; }

        [Cfg()]
        public List<TflEnvironment> Environments { get; set; }
        [Cfg(required = true)]

        public List<TflProcess> Processes { get; set; }
        [Cfg()]
        public List<TflResponse> Response { get; set; }

        public TflRoot(
                string cfg, 
                Dictionary<string, string> parameters = null,
                IDependency logger = null)
            : base(
                  new DefaultReader(new SourceDetector(), new FileReader(), new WebReader()), 
                  logger
            ) {
            Load(cfg, parameters);
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