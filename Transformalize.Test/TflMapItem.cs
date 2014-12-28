namespace Transformalize.Test {
    public class TflMapItem : TflNode {
        public TflMapItem() {
            Key("from");
            Property("operator", "equals");
            Property("parameter", string.Empty);
            Property("to", string.Empty);
        }
    }
}