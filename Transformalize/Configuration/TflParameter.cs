namespace Transformalize.Configuration {

    public class TflParameter : CfgNode {

        public TflParameter() {
            Property("entity", string.Empty);
            Property("field", string.Empty);
            Property("name", string.Empty);
            Property("value", string.Empty);
            Property("input", true);
            Property("type", "string");
        }

        public string Entity { get; set; }
        public string Field { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public bool Input { get; set; }
        public string Type { get; set; }
    }

}