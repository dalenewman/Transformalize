namespace Transformalize.Configuration {
    public class TflFilter : CfgNode {
        public TflFilter() {
            Property(n: "left", v: string.Empty);
            Property(n: "right", v: string.Empty);
            Property(n: "operator", v: "Equal");
            Property(n: "continuation", v: "AND");
            Property(n: "expression", v: string.Empty);
        }

        public string Left { get; set; }
        public string Right { get; set; }
        public string Operator { get; set; }
        public string Continuation { get; set; }
        public string Expression { get; set; }

    }
}