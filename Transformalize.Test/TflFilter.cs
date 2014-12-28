namespace Transformalize.Test {
    public class TflFilter : TflNode {
        public TflFilter() {
            Property("left", string.Empty);
            Property("right", string.Empty);
            Property("operator", "Equal");
            Property("continuation", "AND");
            Property("expression", string.Empty);
        }
    }
}