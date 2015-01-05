using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflFilter : CfgNode {
        public TflFilter() {
            Property(name: "left", value: string.Empty);
            Property(name: "right", value: string.Empty);
            Property(name: "operator", value: "Equal");
            Property(name: "continuation", value: "AND");
            Property(name: "expression", value: string.Empty);
        }

    }
}