using System.Collections.Generic;
using Orchard;
using Transformalize.Configuration;

namespace Pipeline.Web.Orchard.Services.Contracts {
    public interface IBatchRunService : IDependency {
        bool Run(Action action, IDictionary<string, string> parameters);
    }
}