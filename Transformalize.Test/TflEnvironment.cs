using Transformalize.Libs.NanoXml;

namespace Transformalize.Test
{
    public class TflEnvironment : TflNode {
        public TflEnvironment(NanoXmlNode node)
            : base(node) {
            Property("name", string.Empty, true, true);
            Property("default", false);
            Class<TflParameter>("parameters");
            }
    }
}