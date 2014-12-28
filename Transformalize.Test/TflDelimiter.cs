using Transformalize.Libs.NanoXml;

namespace Transformalize.Test {
    public class TflDelimiter : TflNode {
        public TflDelimiter(NanoXmlNode node)
            : base(node) {
            Key("character");
            Property("name", string.Empty, true);
        }
    }
}