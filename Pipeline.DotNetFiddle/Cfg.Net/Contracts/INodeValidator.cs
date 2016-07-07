using System.Collections.Generic;

namespace Cfg.Net.Contracts
{
    public interface INodeValidator : INamedDependency {
        void Validate(INode node, string value, IDictionary<string, string> parameters, ILogger logger);
    }
}