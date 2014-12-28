using Transformalize.Libs.NanoXml;

namespace Transformalize.Test
{
    public class TflFilter : TflNode {
        public TflFilter(NanoXmlNode node)
            : base(node) {
            Property("left", string.Empty);
            Property("right", string.Empty);
            Property("operator", "Equal");
            Property("continuation", "AND");
            Property("expression", string.Empty);
            }
    }
}