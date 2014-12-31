namespace Transformalize.Configuration {
    public class TflJoin : CfgNode {

        public TflJoin() {
            Property(n: "left-field", v: string.Empty, r: true);
            Property(n: "right-field", v: string.Empty, r: true);
        }

        public string LeftField { get; set; }
        public string RightField { get; set; }
    }
}