namespace Transformalize.Configuration {
    public class TflProvider : CfgNode {
        public TflProvider() {
            Property("name", string.Empty, true, true);
            Property("type", string.Empty, true, false);
        }

        public string Name { get; set; }
        public string Type { get; set; }
    }
}