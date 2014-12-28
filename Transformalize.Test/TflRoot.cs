using Transformalize.Libs.NanoXml;

namespace Transformalize.Test {
    public class TflRoot : TflNode {

        public TflRoot(NanoXmlNode node)
            : base(node) {
            Class<TflEnvironment>("environments");
            Class<TflProcess>("processes", true);
        }

        public TflRoot(string xml) : this(new NanoXmlDocument(xml).RootNode) { }
    }
}