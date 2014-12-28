using Transformalize.Libs.NanoXml;

namespace Transformalize.Test {
    public class TflParameter : TflNode {
        public TflParameter(NanoXmlNode node)
            : base(node) {
            Property("entity", string.Empty);
            Property("field", string.Empty);
            Property("name", string.Empty);
            Property("value", string.Empty);
            Property("input", true);
            Property("type", "string");
        }
    }
}