using System.Collections.Generic;
using System.Web.Mvc;
using Orchard;

namespace Pipeline.Web.Orchard.Services.Contracts {
    public interface IBatchRedirectService : IDependency {
        ActionResult Redirect(string url, IDictionary<string, string> parameters);
    }
}