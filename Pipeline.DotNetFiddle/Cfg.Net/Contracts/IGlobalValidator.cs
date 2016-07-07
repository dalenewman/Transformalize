using System.Collections.Generic;

namespace Cfg.Net.Contracts {
    public interface IGlobalValidator : IDependency {
        void Validate(string name, string value, IDictionary<string, string> parameters, ILogger logger);
    }
}