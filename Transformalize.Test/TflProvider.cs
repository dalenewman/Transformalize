namespace Transformalize.Test {
    public class TflProvider : TflNode {
        public TflProvider() {
            Key("name");
            Property("type", string.Empty, true);
        }
    }
}