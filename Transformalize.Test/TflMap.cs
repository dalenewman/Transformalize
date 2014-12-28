namespace Transformalize.Test {
    public class TflMap : TflNode {
        public TflMap() {
            Key("name");
            Property("connection", "input");
            Property("query", string.Empty);
            Class<TflMapItem>("items");
        }
    }
}