using System.Xml.Linq;
using Cfg.Net.Contracts;

namespace Transformalize.Configuration {
    public class XDocumentParser : IParser {
        public INode Parse(string cfg) {
            return new XDocumentNode(XDocument.Parse(cfg).Root);
        }
    }
}