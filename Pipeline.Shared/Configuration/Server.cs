using Cfg.Net;

namespace Transformalize.Configuration {
    public class Server : CfgNode {
        [Cfg(value = "")]
        public string Url { get; set; }

        [Cfg(value = "")]
        public string Name { get; set; }

        [Cfg(value = "")]
        public int Port { get; set; }

        [Cfg(value = "")]
        public string Path { get; set; }

        protected override void Validate() {
            if (Url == string.Empty && Name == string.Empty) {
                Error("A server must have a name or url.");
            }
        }
    }
}