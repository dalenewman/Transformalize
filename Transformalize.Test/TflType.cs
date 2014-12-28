using Transformalize.Libs.NanoXml;

namespace Transformalize.Test {
    public class TflType : TflNode {
        public TflType(NanoXmlNode node)
            : base(node) {
            Key("type");
        }
    }
}