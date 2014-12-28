using Transformalize.Libs.NanoXml;

namespace Transformalize.Test
{
    public class TflRelationship : TflNode {
        public TflRelationship(NanoXmlNode node)
            : base(node) {
            Property("left-entity", string.Empty, true);
            Property("right-entity", string.Empty, true);
            Property("left-field", string.Empty);
            Property("right-field", string.Empty);
            Property("index", false);
            Class<TflJoin>("join");

            }
    }
}