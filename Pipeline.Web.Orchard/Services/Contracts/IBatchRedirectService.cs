using System.Collections.Generic;
using System.Web.Mvc;
using Orchard;
using Transformalize.Configuration;

namespace Pipeline.Web.Orchard.Services.Contracts {
    public interface IBatchRedirectService : IDependency {
        ActionResult Redirect(Process process, IDictionary<string, string> parameters);
    }
}