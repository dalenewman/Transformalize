using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflMapItem : CfgNode {

        public TflMapItem() {
            Property(name: "from", value: string.Empty, required: true, unique: true);
            Property(name: "operator", value: "equals");
            Property(name: "parameter", value: string.Empty);
            Property(name: "to", value: string.Empty);
        }

        public string From { get; set; }
        public string Operator { get; set; }
        public string Parameter { get; set; }
        public string To { get; set; }
    }
}