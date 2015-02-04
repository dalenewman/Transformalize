using System.Collections.Generic;
using Orchard;
using Transformalize.Orchard.Models;

namespace Transformalize.Orchard.Services {
    public interface ITransformalizeService : IDependency {
        IEnumerable<int> FilesCreated { get; }
        void InitializeFiles(TransformalizeRequest request);
        string GetMetaData(TransformalizeRequest request);
        IEnumerable<ConfigurationPart> GetConfigurations();
        IEnumerable<ConfigurationPart> GetAuthorizedConfigurations();
        ConfigurationPart GetConfiguration(int id);
        TransformalizeResponse Run(TransformalizeRequest request);
    }


}



