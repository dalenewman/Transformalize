using System.Collections.Generic;

namespace Cfg.Net.Contracts {
    /// <summary>
    /// A modifier that is run at the root (top-most level) of your configuration.
    /// </summary>
    public interface IRootModifier : IDependency {
        void Modify(INode root, IDictionary<string, string> parameters);
    }
}