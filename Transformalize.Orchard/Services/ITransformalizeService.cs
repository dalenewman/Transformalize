using System.Collections.Generic;
using System.Collections.Specialized;
using Orchard;
using Transformalize.Main;
using Transformalize.Operations;
using Transformalize.Orchard.Models;

namespace Transformalize.Orchard.Services {

    public interface ITransformalizeService : IDependency {
        IEnumerable<int> FilesCreated { get; }
        void InjectParameters(ref ConfigurationPart part, NameValueCollection query);
        string GetMetaData(ConfigurationPart part, NameValueCollection query);
        IEnumerable<ConfigurationPart> GetConfigurations();
        IEnumerable<ConfigurationPart> GetAuthorizedConfigurations();
        ConfigurationPart GetConfiguration(int id);
        TransformalizeResponse Run(ConfigurationPart part, Options options, NameValueCollection query);
    }

    
}



