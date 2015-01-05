using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflScript : CfgNode {

        public TflScript() {
            Property(name: "name", value: string.Empty, required: true, unique: true);
            Property(name: "file", value: string.Empty, required: true);
        }

        public string Name { get; set; }
        public string File { get; set; }
    }
}