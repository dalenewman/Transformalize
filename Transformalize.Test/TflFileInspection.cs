using Transformalize.Libs.NanoXml;

namespace Transformalize.Test {
    public class TflFileInspection : TflNode {
        public TflFileInspection(NanoXmlNode node)
            : base(node) {
            Property("sample", 100);
            Property("max-length", 0);
            Property("min-length", 0);

            Class<TflType>("types");
            Class<TflDelimiter>("delimiters");
        }
    }
}