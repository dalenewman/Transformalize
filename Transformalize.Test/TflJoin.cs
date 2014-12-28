namespace Transformalize.Test {
    public class TflJoin : TflNode {
        public TflJoin() {
            Property("left-field", string.Empty, true);
            Property("right-field", string.Empty, true);
        }
    }
}