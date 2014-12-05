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
        private readonly IApiService _apiService;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public Localizer T { get; set; }

        public ApiController(
            IOrchardServices services,
            ITransformalizeService transformalize,
            IApiService apiService
        ) {
            _stopwatch.Start();
            _orchardServices = services;
            _transformalize = transformalize;
            _apiService = apiService;
            T = NullLocalizer.Instance;
        }

        [ActionName("Api/Configuration")]
        public ActionResult Configuration(int id) {

            Response.AddHeader("Access-Control-Allow-Origin", "*");
            var request = new ApiRequest(ApiRequestType.Configuration) { Stopwatch = _stopwatch };
            var query = GetQuery();

            var part = _orchardServices.ContentManager.Get(id).As<ConfigurationPart>();
            if (part == null) {
                return _apiService.NotFound(request, query);
            }

            if (!(Request.IsLocal || part.IsInAllowedRange(Request.UserHostAddress))) {
                if (User.Identity.IsAuthenticated) {
                    if (!_orchardServices.Authorizer.Authorize(global::Orchard.Core.Contents.Permissions.ViewContent, part)) {
                        return _apiService.Unathorized(request, query);
                    }
                } else {
                    return _apiService.Unathorized(request, query);
                }
            }

            var configuration = _transformalize.InjectParameters(part, GetQuery());

            return new ApiResponse(request, configuration).ContentResult(
                GetQuery()["format"] ?? DEFAULT_FORMAT,
                GetQuery()["flavor"] ?? DEFAULT_FLAVOR
            );
        }

        [ActionName("Api/MetaData")]
        public ActionResult MetaData(int id) {
            Response.AddHeader("Access-Control-Allow-Origin", "*");

            var request = new ApiRequest(ApiRequestType.MetaData) { Stopwatch = _stopwatch };
            var query = GetQuery();

            if (!User.Identity.IsAuthenticated) {
                return _apiService.Unathorized(request, query);
            }

            var part = _orchardServices.ContentManager.Get(id).As<ConfigurationPart>();
            if (part == null) {
                return _apiService.NotFound(request, query);
            }

            var configuration = _transformalize.InjectParameters(part, query);

            return new ApiResponse(request, configuration, _transformalize.GetMetaData(configuration)).ContentResult(
                query["format"] ?? DEFAULT_FORMAT,
                query["flavor"] ?? DEFAULT_FLAVOR
            );
        }


        [ActionName("Api/Execute"), ValidateInput(false)]
        public ActionResult Execute(int id) {

            Response.AddHeader("Access-Control-Allow-Origin", "*");
            var request = new ApiRequest(ApiRequestType.Execute) { Stopwatch = _stopwatch };
            var query = GetQuery();
            var configuration = Request.Form["configuration"];

            if (id == 0 && configuration == null) {
                return _apiService.NotFound(request, query);
            }

            if (!Request.IsLocal) {
                if (!User.Identity.IsAuthenticated)
                    return _apiService.Unathorized(request, query);
                if (!_orchardServices.Authorizer.Authorize(Permissions.Execute))
                    return _apiService.Unathorized(request, query);
            }

            ConfigurationPart part;
            if (id == 0) {
                part = _orchardServices.ContentManager.New<ConfigurationPart>("Configuration");
                part.Configuration = configuration;
            } else {
                part = _orchardServices.ContentManager.Get(id).As<ConfigurationPart>();
            }

            if (part == null) {
                return _apiService.NotFound(request, query);
            }

            if (!Request.IsLocal) {
                if (!_orchardServices.Authorizer.Authorize(global::Orchard.Core.Contents.Permissions.ViewContent, part)) {
                    return _apiService.Unathorized(request, query);
                }
            }

            // ready
            var transformalizeRequest = new TransformalizeRequest(part) {
                Configuration = _transformalize.InjectParameters(part, query),
                Options = query["Mode"] != null ? new Options { Mode = query["Mode"] } : new Options(),
                Query = query
            };

            var processes = new TransformalizeResponse();

            try {
                processes = _transformalize.Run(transformalizeRequest);
            } catch (Exception ex) {
                request.Status = 500;
                request.Message = ex.Message;
            }

            return new ApiResponse(request, transformalizeRequest.Configuration, processes).ContentResult(
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