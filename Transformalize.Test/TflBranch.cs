using Transformalize.Main;

namespace Transformalize.Test {
    public class TflBranch : TflNode {
        public TflBranch() {
            Key("name");

            Property("run-field", Common.DefaultValue);
            Property("run-operator", "Equal");
            Property("run-type", Common.DefaultValue);
            Property("run-value", string.Empty);

            Class<TflTransform>("transforms");
        }
    }
}