using System.Xml.Linq;
using Transformalize.Libs.Cfg.Net.Parsers;

namespace Transformalize.Configuration {
    public class XDocumentParser : IParser {
        public INode Parse(string cfg) {
            return new XDocumentNode(XDocument.Parse(cfg).Root);
        }
    }
}