using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflJoin : CfgNode {

        public TflJoin() {
            Property(name: "left-field", value: string.Empty, required: true);
            Property(name: "right-field", value: string.Empty, required: true);
        }

    }
}