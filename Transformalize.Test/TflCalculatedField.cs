using Transformalize.Libs.NanoXml;

namespace Transformalize.Test
{
    public class TflCalculatedField : TflField {
        public TflCalculatedField(NanoXmlNode node)
            : base(node) {
            Property("input", false);
            Property("alias", string.Empty);
            }
    }
}