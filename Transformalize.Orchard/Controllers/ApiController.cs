using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Logging;
using Transformalize.Extensions;
using Transformalize.Main;
using Transformalize.Orchard.Models;
using Transformalize.Orchard.Services;

namespace Transformalize.Orchard.Controllers {
    public class ApiController : TflController {

        private static readonly string OrchardVersion = typeof(ContentItem).Assembly.GetName().Version.ToString();
        private readonly ITransformalizeService _transformalize;
        private readonly IApiService _apiService;
        private readonly string _moduleVersion;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ApiController(
            ITransformalizeService transformalize,
            IApiService apiService,
            IExtensionManager extensionManager
        ) {
            _stopwatch.Start();
            _transformalize = transformalize;
            _apiService = apiService;
            _moduleVersion = extensionManager.GetExtension("Transformalize.Orchard").Version;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
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
            var transformalizeRequest = new TransformalizeRequest(part, query, null, Logger);
            var logger = new TransformalizeLogger(transformalizeRequest.Part.Title(), part.LogLevel, Logger, OrchardVersion, _moduleVersion);

            var errors = transformalizeRequest.Root.Errors();
            if (errors.Any()) {
                var bad = new TransformalizeResponse();
                request.Status = 501;
                request.Message = "Configuration Problem" + errors.Length.Plural();
                bad.Log.AddRange(errors.Select(p => new[]{DateTime.Now.ToString(CultureInfo.InvariantCulture),"error",".",".",p}));
                return new ApiResponse(request, "<tfl></tfl>", bad).ContentResult(
                    query["format"] ?? DefaultFormat,
                    query["flavor"] ?? DefaultFlavor
                );
            }

            var metaData = new MetaDataWriter(ProcessFactory.CreateSingle(transformalizeRequest.Root.Processes[0], logger, new Options { Mode = "metadata" })).Write();
            return new ApiResponse(request, metaData).ContentResult(
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

            var query = GetQuery();

            _transformalize.InitializeFiles(part, query);

            // ready
            var processes = new TransformalizeResponse();
            var transformalizeRequest = new TransformalizeRequest(part, query, null, Logger);

            try {
                processes = _transformalize.Run(transformalizeRequest);
            } catch (Exception ex) {
                request.Status = 500;
                request.Message = ex.Message + " " + WebUtility.HtmlEncode(ex.StackTrace);
            }

            return new ApiResponse(request, transformalizeRequest.Configuration, processes).ContentResult(
                transformalizeRequest.Query["format"],
                transformalizeRequest.Query["flavor"]
            );
        }

    }
}