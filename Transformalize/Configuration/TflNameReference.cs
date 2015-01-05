using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflNameReference : CfgNode {
        public TflNameReference() {
            Property(name: "name", value: string.Empty, required: true, unique: true);
        }

    }
}