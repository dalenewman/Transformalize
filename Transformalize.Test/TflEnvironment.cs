namespace Transformalize.Test {
    public class TflEnvironment : TflNode {
        public TflEnvironment() {
            Property("name", string.Empty, true, true);
            Property("default", false);
            Class<TflParameter>("parameters");
        }
    }
}