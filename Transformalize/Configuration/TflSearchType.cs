using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflSearchType : CfgNode {
        public TflSearchType() {
            Property(name: "name", value: string.Empty, required: true, unique: true);
            Property(name: "store", value: true);
            Property(name: "index", value: true);
            Property(name: "multi-valued", value: false);
            Property(name: "analyzer", value: string.Empty);
            Property(name: "norms", value: true);
        }

        public string Name { get; set; }
        public bool Store { get; set; }
        public bool Index { get; set; }
        public bool MultiValued { get; set; }
        public string Analyzer { get; set; }
        public bool Norms { get; set; }
    }
}