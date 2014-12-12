using System.Collections.Generic;
using System.Diagnostics;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Logging;
using Transformalize.Orchard.Models;

namespace Transformalize.Orchard.Services {

    public class ApiService : IApiService {
        private readonly IOrchardServices _orchardServices;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        protected ILogger Logger { get; set; }

        public ApiService(IOrchardServices orchardServices) {
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _stopwatch.Start();
        }

        public ApiResponse NotFound(ApiRequest request) {
            request.Status = 404;
            request.Message = "Not Found";
            return new ApiResponse(request, "<transformalize><processes></processes></transformalize>");
        }

        public ApiResponse Unathorized(ApiRequest request) {
            request.Status = 401;
            request.Message = "Unauthorized";
            return new ApiResponse(request, "<transformalize><processes></processes></transformalize>");
        }

        public List<ApiResponse> Rejections(int id, out ApiRequest request, out ConfigurationPart part) {

            var context = _orchardServices.WorkContext.HttpContext;
            request = new ApiRequest(ApiRequestType.Configuration) { Stopwatch = _stopwatch };
            var response = new List<ApiResponse>();

            if (id == 0) {
                var configuration = context.Request.Form["configuration"];
                if (configuration != null) {
                    part = _orchardServices.ContentManager.New<ConfigurationPart>("Configuration");
                    part.Configuration = configuration;
                } else {
                    part = null;
                }
            } else {
                part = _orchardServices.ContentManager.Get(id).As<ConfigurationPart>();
            }

            if (part == null) {
                Logger.Error("No Configuration for id {0}.  Requested by {1} at {2}.", id, context.User.Identity.IsAuthenticated ? context.User.Identity.Name : "Anonymous", context.Request.UserHostAddress);
                response.Add(NotFound(request));
                return response;
            }

            if (context.Request.IsLocal || part.IsInAllowedRange(context.Request.UserHostAddress)) {
                return response;
            }

            if (context.User.Identity.IsAuthenticated) {
                if (_orchardServices.Authorizer.Authorize(global::Orchard.Core.Contents.Permissions.ViewContent, part)) {
                    return response;
                }
                Logger.Error("Not authorized to run {0}.  Requested by {1} at {2}.", part.As<TitlePart>().Title, context.User.Identity.Name, context.Request.UserHostAddress);
                response.Add(Unathorized(request));
                return response;
            }

            Logger.Error("Not authorized to run {0}.  Requested by Anonymous at {1}.", part.As<TitlePart>().Title, context.Request.UserHostAddress);
            response.Add(Unathorized(request));
            return response;
        }
    }
}