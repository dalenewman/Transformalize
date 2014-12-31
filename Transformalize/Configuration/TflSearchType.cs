namespace Transformalize.Configuration {
    public class TflSearchType : CfgNode {
        public TflSearchType() {
            Property(n: "name", v: string.Empty, r: true, u: true);
            Property(n: "store", v: true);
            Property(n: "index", v: true);
            Property(n: "multi-valued", v: false);
            Property(n: "analyzer", v: string.Empty);
            Property(n: "norms", v: true);
        }

        public string Name { get; set; }
        public bool Store { get; set; }
        public bool Index { get; set; }
        public bool MultiValued { get; set; }
        public string Analyzer { get; set; }
        public bool Norms { get; set; }
    }
}