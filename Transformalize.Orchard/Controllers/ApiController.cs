using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Transformalize.Main;
using Transformalize.Orchard.Models;
using Transformalize.Orchard.Services;

namespace Transformalize.Orchard.Controllers {
    public class ApiController : Controller {

        private const string DEFAULT_FORMAT = "xml";
        private const string DEFAULT_FLAVOR = "attributes";

        private readonly IOrchardServices _orchardServices;
        private readonly ITransformalizeService _transformalize;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public Localizer T { get; set; }

        public ApiController(
            IOrchardServices services,
            ITransformalizeService transformalize
        ) {
            _stopwatch.Start();
            _orchardServices = services;
            _transformalize = transformalize;
            T = NullLocalizer.Instance;
        }

        private static ActionResult SimpleResult(ApiRequest request, int status, string message) {
            request.Status = status;
            request.Message = message;
            var query = GetQuery();
            return new ApiResponse(request, "<transformalize><processes></processes></transformalize>").ContentResult(
                query["format"] ?? DEFAULT_FORMAT,
                query["flavor"] ?? DEFAULT_FLAVOR
            );
        }

        [ActionName("Api/Configuration")]
        public ActionResult Configuration(int id) {

            Response.AddHeader("Access-Control-Allow-Origin", "*");
            var request = new ApiRequest(ApiRequestType.Configuration) { Stopwatch = _stopwatch };

            var part = _orchardServices.ContentManager.Get(id).As<ConfigurationPart>();
            if (part == null) {
                return SimpleResult(request, 404, "Not Found");
            }

            if (!(Request.IsLocal || part.IsInAllowedRange(Request.UserHostAddress))) {
                if (User.Identity.IsAuthenticated) {
                    if (!_orchardServices.Authorizer.Authorize(global::Orchard.Core.Contents.Permissions.ViewContent, part)) {
                        return SimpleResult(request, 401, "Unauthorized");
                    }
                } else {
                    return SimpleResult(request, 404, "Not Found");
                }
            }

            _transformalize.InjectParameters(ref part, GetQuery());
            request.Configuration = part.Configuration;

            return new ApiResponse(request).ContentResult(
                GetQuery()["format"] ?? DEFAULT_FORMAT,
                GetQuery()["flavor"] ?? DEFAULT_FLAVOR
            );
        }

        [ActionName("Api/MetaData")]
        public ActionResult MetaData(int id) {
            Response.AddHeader("Access-Control-Allow-Origin", "*");

            var request = new ApiRequest(ApiRequestType.MetaData) { Stopwatch = _stopwatch };

            if (!User.Identity.IsAuthenticated) {
                return SimpleResult(request, 401, "Unauthorized");
            }

            var part = _orchardServices.ContentManager.Get(id).As<ConfigurationPart>();
            if (part == null) {
                return SimpleResult(request, 404, "Not Found");
            }

            var query = GetQuery();
            _transformalize.InjectParameters(ref part, query);
            request.Configuration = part.Configuration;

            return new ApiResponse(request, _transformalize.GetMetaData(part, query)).ContentResult(
                query["format"] ?? DEFAULT_FORMAT,
                query["flavor"] ?? DEFAULT_FLAVOR
            );
        }

        [ActionName("Api/Execute")]
        public ActionResult Execute(int id) {

            Response.AddHeader("Access-Control-Allow-Origin", "*");
            var request = new ApiRequest(ApiRequestType.Execute) { Stopwatch = _stopwatch };

            if (id == 0)
                return SimpleResult(request, 404, "Not Found");

            if (!Request.IsLocal) {
                if (!User.Identity.IsAuthenticated)
                    return SimpleResult(request, 401, "Unauthorized");
                if (!_orchardServices.Authorizer.Authorize(Permissions.Execute))
                    return SimpleResult(request, 401, "Unauthorized");
            }

            var part = _orchardServices.ContentManager.Get(id).As<ConfigurationPart>();
            if (part == null)
                return SimpleResult(request, 404, "Not Found");

            if (!Request.IsLocal) {
                if (!_orchardServices.Authorizer.Authorize(global::Orchard.Core.Contents.Permissions.ViewContent, part)) {
                    return SimpleResult(request, 401, "Unauthorized");
                }
            }

            // ready
            var query = GetQuery();
            _transformalize.InjectParameters(ref part, query);
            request.Configuration = part.Configuration;

            var options = query["Mode"] != null ? new Options { Mode = query["Mode"] } : new Options();

            var processes = new TransformalizeResponse();

            try {
                processes = _transformalize.Run(part, options, query);
            } catch (Exception ex) {
                request.Status = 500;
                request.Message = ex.Message;
            }

            return new ApiResponse(request, processes).ContentResult(
                query["format"] ?? DEFAULT_FORMAT,
                query["flavor"] ?? DEFAULT_FLAVOR
            );
        }

        private static NameValueCollection GetQuery() {
            var request = System.Web.HttpContext.Current.Request;
            return new NameValueCollection { request.Form, request.QueryString };
        }

    }
}