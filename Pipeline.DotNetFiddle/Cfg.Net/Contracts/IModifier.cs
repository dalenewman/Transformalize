using System.Collections.Generic;

namespace Cfg.Net.Contracts {

    public interface IModifier : INamedDependency {
        object Modify(string name, object value, IDictionary<string, string> parameters);
    }
}