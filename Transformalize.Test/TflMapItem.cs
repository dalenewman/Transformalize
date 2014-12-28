using Transformalize.Libs.NanoXml;

namespace Transformalize.Test
{
    public class TflMapItem : TflNode {
        public TflMapItem(NanoXmlNode node)
            : base(node) {
            Key("from");
            Property("operator", "equals");
            Property("parameter", string.Empty);
            Property("to", string.Empty);
            }
    }
}