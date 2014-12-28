using Transformalize.Libs.NanoXml;

namespace Transformalize.Test
{
    public class TflProvider : TflNode {
        public TflProvider(NanoXmlNode node)
            : base(node) {
            Key("name");
            Property("type", string.Empty, true);
            }
    }
}