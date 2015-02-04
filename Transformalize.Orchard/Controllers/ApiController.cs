using System;
using System.Diagnostics;
using System.Web.Mvc;
using Orchard.Localization;
using Transformalize.Orchard.Models;
using Transformalize.Orchard.Services;

namespace Transformalize.Orchard.Controllers {
    public class ApiController : TflController {

        private readonly ITransformalizeService _transformalize;
        private readonly IApiService _apiService;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public Localizer T { get; set; }

        public ApiController(
            ITransformalizeService transformalize,
            IApiService apiService
        ) {
            _stopwatch.Start();
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
                    DefaultFormat,
                    DefaultFlavor
                );
            }

            var query = GetQuery();

            return new ApiResponse(request, part.Configuration).ContentResult(
                query["format"] ?? DefaultFormat,
                query["flavor"] ?? DefaultFlavor
            );
        }

        [ActionName("Api/MetaData")]
        public ActionResult MetaData(int id) {

            Response.AddHeader("Access-Control-Allow-Origin", "*");
            ConfigurationPart part;
            ApiRequest request;

            foreach (var rejection in _apiService.Rejections(id, out request, out part)) {
                return rejection.ContentResult(
                    DefaultFormat,
                    DefaultFlavor
                );
            }

            request.RequestType = ApiRequestType.MetaData;
            var query = GetQuery();
            var transformalizeRequest = new TransformalizeRequest(part, query, null);

            return new ApiResponse(request, part.Configuration, _transformalize.GetMetaData(transformalizeRequest)).ContentResult(
                query["format"] ?? DefaultFormat,
                query["flavor"] ?? DefaultFlavor
            );
        }


        [ActionName("Api/Execute"), ValidateInput(false)]
        public ActionResult Execute(int id) {

            Response.AddHeader("Access-Control-Allow-Origin", "*");
            ConfigurationPart part;
            ApiRequest request;

            foreach (var rejection in _apiService.Rejections(id, out request, out part)) {
                return rejection.ContentResult(
                    DefaultFormat,
                    DefaultFlavor
                );
            }

            request.RequestType = ApiRequestType.Execute;

            // ready
            var transformalizeRequest = new TransformalizeRequest(part, GetQuery(), null);

            _transformalize.InitializeFiles(transformalizeRequest);

            var processes = new TransformalizeResponse();

            try {
                processes = _transformalize.Run(transformalizeRequest);
            } catch (Exception ex) {
                request.Status = 500;
                request.Message = ex.Message + " " + ex.StackTrace;
            }

            return new ApiResponse(request, transformalizeRequest.Configuration, processes).ContentResult(
                transformalizeRequest.Query["format"],
                transformalizeRequest.Query["flavor"]
            );
        }

    }
}