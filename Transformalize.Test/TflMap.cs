using Transformalize.Libs.NanoXml;

namespace Transformalize.Test
{
    public class TflMap : TflNode {
        public TflMap(NanoXmlNode node)
            : base(node) {
            Key("name");
            Property("connection", "input");
            Property("query", string.Empty);
            Class<TflMapItem>("items");
            }
    }
}