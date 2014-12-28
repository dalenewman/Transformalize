namespace Transformalize.Test {
    public class TflScript : TflNode {
        public TflScript() {
            Key("name");
            Property("file", string.Empty, true);
        }
    }
}