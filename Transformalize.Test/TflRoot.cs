namespace Transformalize.Test {
    public class TflRoot : TflNode {

        public TflRoot() {
            Class<TflEnvironment>("environments");
            Class<TflProcess>("processes", true);
        }
    }
}