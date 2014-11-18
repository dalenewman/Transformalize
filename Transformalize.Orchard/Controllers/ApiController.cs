using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Transformalize.Main;
using Transformalize.Orchard.Models;
using Transformalize.Orchard.Services;
using Process = Transformalize.Main.Process;

namespace Transformalize.Orchard.Controllers {
    public class ApiController : Controller {

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

        private static ActionResult SimpleResult(ApiRequest request, int status, string message = "") {
            return new ApiResponse(request, "<transformalize><processes></processes></transformalize>") {
                Status = status,
                Message = message
            }.ContentResult(GetQuery()["format"] ?? "json");
        }

        [ActionName("Api/Configuration")]
        public ActionResult Configuration(int id) {
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
            var query = GetQuery();
            request.Configuration = _transformalize.InjectParameters(part, query);
            request.Query = query;
            return new ApiResponse(request).ContentResult(query["format"] ?? "json");
        }

        [ActionName("Api/MetaData")]
        public ActionResult MetaData(int id) {
            var request = new ApiRequest(ApiRequestType.MetaData) { Stopwatch = _stopwatch };

            if (!User.Identity.IsAuthenticated) {
                return SimpleResult(request, 401, "Unauthorized");
            }

            var part = _orchardServices.ContentManager.Get(id).As<ConfigurationPart>();
            if (part == null) {
                return SimpleResult(request, 404, "Not Found");
            }

            var query = GetQuery();
            request.Configuration = _transformalize.InjectParameters(part, query);
            request.Query = query;
            return new ApiResponse(request, _transformalize.GetMetaData(part, query)).ContentResult(query["format"] ?? "json");
        }

        [ActionName("Api/Execute")]
        public ActionResult Execute(int id) {
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
            request.Configuration = _transformalize.InjectParameters(part, query);
            request.Query = query;
            var options = query["Mode"] != null ? new Options { Mode = query["Mode"] } : new Options();

            var processes = new Process[0];
            var message = string.Empty;
            var status = 200;

            try {
                processes = RunCommon(request.Configuration, options);
            } catch (Exception ex) {
                status = 500;
                message = ex.Message;
            }

            return new ApiResponse(request, processes) {
                Status = status,
                Message = message
            }.ContentResult(GetQuery()["format"] ?? "json");
        }

        private static NameValueCollection GetQuery() {
            var request = System.Web.HttpContext.Current.Request;
            return new NameValueCollection {request.Form, request.QueryString};
        }

        private static Process[] RunCommon(string configuration, Options options) {
            var processes = new List<Process>();
            if (options.Mode.Equals("rebuild", StringComparison.OrdinalIgnoreCase)) {
                options.Mode = "init";
                processes.AddRange(ProcessFactory.Create(configuration, options));
                options.Mode = "first";
                processes.AddRange(ProcessFactory.Create(configuration, options));
            } else {
                processes.AddRange(ProcessFactory.Create(configuration, options));
            }

            foreach (var process in processes) {
                process.ExecuteScaler();
            }

            return processes.ToArray();
        }

    }
}