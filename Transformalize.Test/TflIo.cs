using Transformalize.Libs.NanoXml;
using Transformalize.Main;

namespace Transformalize.Test
{
    public class TflIo : TflNode {
        public TflIo(NanoXmlNode node)
            : base(node) {
            Property("name", string.Empty);
            Property("connection", string.Empty, true);
            Property("run-field", string.Empty);
            Property("run-type", Common.DefaultValue);
            Property("run-operator", "Equal");
            Property("run-value", string.Empty);
            }
    }
}