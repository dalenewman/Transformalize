using Transformalize.Libs.NanoXml;

namespace Transformalize.Test
{
    public class TflSearchType : TflNode {
        public TflSearchType(NanoXmlNode node)
            : base(node) {
            Key("name");
            Property("store", true);
            Property("index", true);
            Property("multi-valued", false);
            Property("analyzer", string.Empty);
            Property("norms", true);
            }
    }
}