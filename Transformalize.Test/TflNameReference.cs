using Transformalize.Libs.NanoXml;

namespace Transformalize.Test
{
    public class TflNameReference : TflNode {
        public TflNameReference(NanoXmlNode node)
            : base(node) {
            Key("name");
            }
    }
}