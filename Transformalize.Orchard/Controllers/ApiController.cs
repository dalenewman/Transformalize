using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Web.Mvc;
using Orchard;
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
            ConfigurationPart part;
            ApiRequest request;

            foreach (var rejection in _apiService.Rejections(id, out request, out part)) {
                return rejection.ContentResult(
                    DEFAULT_FORMAT,
                    DEFAULT_FLAVOR
                );
            }

            var query = GetQuery();
            var configuration = _transformalize.InjectParameters(part, query);

            return new ApiResponse(request, configuration).ContentResult(
                query["format"] ?? DEFAULT_FORMAT,
                query["flavor"] ?? DEFAULT_FLAVOR
            );
        }

        [ActionName("Api/MetaData")]
        public ActionResult MetaData(int id) {

            Response.AddHeader("Access-Control-Allow-Origin", "*");
            ConfigurationPart part;
            ApiRequest request;

            foreach (var rejection in _apiService.Rejections(id, out request, out part)) {
                return rejection.ContentResult(
                    DEFAULT_FORMAT,
                    DEFAULT_FLAVOR
                );
            }

            request.RequestType = ApiRequestType.MetaData;
            var query = GetQuery();
            var configuration = _transformalize.InjectParameters(part, query);

            return new ApiResponse(request, configuration, _transformalize.GetMetaData(configuration)).ContentResult(
                query["format"] ?? DEFAULT_FORMAT,
                query["flavor"] ?? DEFAULT_FLAVOR
            );
        }


        [ActionName("Api/Execute"), ValidateInput(false)]
        public ActionResult Execute(int id) {

            Response.AddHeader("Access-Control-Allow-Origin", "*");
            ConfigurationPart part;
            ApiRequest request;

            foreach (var rejection in _apiService.Rejections(id, out request, out part)) {
                return rejection.ContentResult(
                    DEFAULT_FORMAT,
                    DEFAULT_FLAVOR
                );
            }

            request.RequestType = ApiRequestType.Execute;
            var query = GetQuery();

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