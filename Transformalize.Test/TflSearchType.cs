namespace Transformalize.Test {
    public class TflSearchType : TflNode {
        public TflSearchType() {
            Key("name");
            Property("store", true);
            Property("index", true);
            Property("multi-valued", false);
            Property("analyzer", string.Empty);
            Property("norms", true);
        }
    }
}