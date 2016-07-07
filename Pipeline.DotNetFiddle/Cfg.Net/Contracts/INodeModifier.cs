using System.Collections.Generic;

namespace Cfg.Net.Contracts {
    public interface INodeModifier : INamedDependency {
        void Modify(INode node, object value, IDictionary<string, string> parameters);
    }
}