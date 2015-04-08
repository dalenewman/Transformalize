using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflScript : CfgNode {

        [Cfg(required = true, unique = true)]
        public string Name { get; set; }
        
        [Cfg(required = true)]
        public string File { get; set; }
        
        [Cfg(value="")]
        public string Path { get; set; }

        [Cfg(value="")]
        public string Script { get; set; }

    }
}