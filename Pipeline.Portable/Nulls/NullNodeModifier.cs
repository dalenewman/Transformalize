using System.Collections.Generic;
using Cfg.Net.Contracts;

namespace Pipeline.Nulls {
    public class NullNodeModifier : INodeModifier {

        public NullNodeModifier(string name) {
            Name = name;
        }
        public string Name { get; set; }
        public void Modify(INode node, string value, IDictionary<string, string> parameters) { }
    }
}