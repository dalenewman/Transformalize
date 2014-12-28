namespace Transformalize.Test {
    public class TflParameter : TflNode {
        public TflParameter() {
            Property("entity", string.Empty);
            Property("field", string.Empty);
            Property("name", string.Empty);
            Property("value", string.Empty);
            Property("input", true);
            Property("type", "string");
        }
    }
}