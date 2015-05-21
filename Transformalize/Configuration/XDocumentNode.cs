using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Transformalize.Libs.Cfg.Net.Parsers;

namespace Transformalize.Configuration {

    public class XDocumentNode : INode {

        public string Name { get; private set; }
        public List<IAttribute> Attributes { get; private set; }
        public List<INode> SubNodes { get; private set; }

        public XDocumentNode(XElement node) {
            Name = node.Name.LocalName;
            Attributes = new List<IAttribute>(node.Attributes().Select(a => new NodeAttribute() { Name = a.Name.LocalName, Value = a.Value }));
            SubNodes = new List<INode>(node.Elements().Select(n => new XDocumentNode(n)));
        }

        public bool TryAttribute(string name, out IAttribute attr) {
            if (Attributes.Any()) {
                if (Attributes.Exists(a => a.Name == name)) {
                    attr = Attributes.First(a => a.Name == name);
                    return true;
                }
            }
            attr = null;
            return false;
        }
    }
}