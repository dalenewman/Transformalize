using Transformalize.Libs.NanoXml;

namespace Transformalize.Test
{
    public class TflJoin : TflNode {
        public TflJoin(NanoXmlNode node)
            : base(node) {
            Property("left-field", string.Empty, true);
            Property("right-field", string.Empty, true);
            }
    }
}