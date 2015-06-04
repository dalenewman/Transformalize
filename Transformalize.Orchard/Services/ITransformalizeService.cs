using System.Collections.Generic;
using Orchard;
using Transformalize.Orchard.Models;

namespace Transformalize.Orchard.Services {
    public interface ITransformalizeService : IDependency {
        IEnumerable<int> FilesCreated { get; }
        void InitializeFiles(ConfigurationPart part, IDictionary<string, string> query);
        IEnumerable<ConfigurationPart> GetConfigurations();
        IEnumerable<ConfigurationPart> GetAuthorizedConfigurations();
        ConfigurationPart GetConfiguration(int id);
        TransformalizeResponse Run(TransformalizeRequest request);
        Dictionary<string, string> GetQuery();
    }
}



