using Transformalize.Libs.Cfg.Net;
using Transformalize.Main;

namespace Transformalize.Configuration {
    public class TflIo : CfgNode {
        public TflIo() {
            Property(name: "name", value: string.Empty);
            Property(name: "connection", value: string.Empty, required: true);
            Property(name: "run-field", value: string.Empty);
            Property(name: "run-type", value: Common.DefaultValue);
            Property(name: "run-operator", value: "Equal");
            Property(name: "run-value", value: string.Empty);
        }

    }
}