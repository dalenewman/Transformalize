namespace Transformalize.Configuration {
    public class TflScript : CfgNode {

        public TflScript() {
            Property(n: "name", v: string.Empty, r: true, u: true);
            Property(n: "file", v: string.Empty, r: true);
        }

        public string Name { get; set; }
        public string File { get; set; }
    }
}