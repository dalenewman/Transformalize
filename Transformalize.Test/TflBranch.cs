using Transformalize.Libs.NanoXml;
using Transformalize.Main;

namespace Transformalize.Test
{
    public class TflBranch : TflNode {
        public TflBranch(NanoXmlNode node)
            : base(node) {
            Key("name");

            Property("run-field", Common.DefaultValue);
            Property("run-operator", "Equal");
            Property("run-type", Common.DefaultValue);
            Property("run-value", string.Empty);

            Class<TflTransform>("transforms");
            }
    }
}