using System.Collections.Generic;
using Orchard;
using Transformalize.Configuration;

namespace Pipeline.Web.Orchard.Services.Contracts {
    public interface IBatchCreateService : IDependency {
        IDictionary<string, string> Create(Process process, IDictionary<string,string> parameters);
    }
}