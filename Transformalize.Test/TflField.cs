using Transformalize.Libs.NanoXml;
using Transformalize.Main;

namespace Transformalize.Test
{
    public class TflField : TflNode {
        public TflField(NanoXmlNode node)
            : base(node) {
            Property("aggregate", string.Empty);
            Property("alias", string.Empty, false, true);
            Property("default", string.Empty);
            Property("default-blank", false);
            Property("default-empty", false);
            Property("default-white-space", false);
            Property("delimiter", ", ");
            Property("distinct", false);
            Property("index", short.MaxValue);
            Property("input", true);
            Property("label", string.Empty);
            Property("length", "64");
            Property("name", string.Empty, true);
            Property("node-type", "element");
            Property("optional", false);
            Property("output", true);
            Property("precision", 18);
            Property("primary-key", false);
            Property("quoted-with", default(char));
            Property("raw", false);
            Property("read-inner-xml", true);
            Property("scale", 9);
            Property("search-type", "default");
            Property("sort", string.Empty);
            Property("t", string.Empty);
            Property("type", "string");
            Property("unicode", Common.DefaultValue);
            Property("variable-length", Common.DefaultValue);

            Class<TflNameReference>("search-types");
            Class<TflTransform>("transforms");
            }
    }
}