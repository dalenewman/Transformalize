using Transformalize.Libs.NanoXml;

namespace Transformalize.Test
{
    public class TflScript : TflNode {
        public TflScript(NanoXmlNode node)
            : base(node) {
            Key("name");
            Property("file", string.Empty, true);
            }
    }
}