using System.Collections.Specialized;
using System.Web.Mvc;
using Orchard;
using Transformalize.Orchard.Models;

namespace Transformalize.Orchard.Services {
    public interface IApiService : IDependency {
        ActionResult NotFound(ApiRequest request, NameValueCollection query);
        ActionResult Unathorized(ApiRequest request, NameValueCollection query);
    }
}