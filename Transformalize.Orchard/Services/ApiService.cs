using System.Collections.Specialized;
using System.Web.Mvc;
using Transformalize.Orchard.Models;

namespace Transformalize.Orchard.Services {

    public class ApiService : IApiService {

        private const string DEFAULT_FORMAT = "xml";
        private const string DEFAULT_FLAVOR = "attributes";

        public ActionResult NotFound(ApiRequest request, NameValueCollection query) {
            request.Status = 404;
            request.Message = "Not Found";
            return new ApiResponse(request, "<transformalize><processes></processes></transformalize>").ContentResult(
                query["format"] ?? DEFAULT_FORMAT,
                query["flavor"] ?? DEFAULT_FLAVOR
                );
        }

        public ActionResult Unathorized(ApiRequest request, NameValueCollection query) {
            request.Status = 401;
            request.Message = "Unauthorized";
            return new ApiResponse(request, "<transformalize><processes></processes></transformalize>").ContentResult(
                query["format"] ?? DEFAULT_FORMAT,
                query["flavor"] ?? DEFAULT_FLAVOR
                );
        }
    }
}