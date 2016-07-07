using System.Collections;

namespace Cfg.Net.Contracts {
    public interface INamedDependency : IDependency {
        string Name { get; set; }
    }
}