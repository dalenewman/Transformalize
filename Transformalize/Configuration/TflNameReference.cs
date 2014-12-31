namespace Transformalize.Configuration {
    public class TflNameReference : CfgNode {
        public TflNameReference() {
            Property(n: "name", v: string.Empty, r: true, u: true);
        }

        public string Name { get; set; }
    }
}