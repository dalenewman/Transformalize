namespace Transformalize.Configuration {
    public class TflMapItem : CfgNode {

        public TflMapItem() {
            Property(n: "from", v: string.Empty, r: true, u: true);
            Property(n: "operator", v: "equals");
            Property(n: "parameter", v: string.Empty);
            Property(n: "to", v: string.Empty);
        }

        public string From { get; set; }
        public string Operator { get; set; }
        public string Parameter { get; set; }
        public string To { get; set; }
    }
}