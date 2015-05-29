using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Themes;
using Transformalize.Configuration;
using Transformalize.Libs.Cfg.Net.Loggers;
using Transformalize.Orchard.Models;
using Transformalize.Orchard.Services;

namespace Transformalize.Orchard.Controllers {

    [Themed]
    public class HandsOnTableController : WidgetController {

        private readonly IOrchardServices _orchardServices;
        private readonly IApiService _apiService;
        private readonly Dictionary<string, string> _query = new Dictionary<string, string>(3);

        public HandsOnTableController(
            IOrchardServices services,
            ITransformalizeService transformalize,
            IApiService apiService
        )
            : base(transformalize) {
            _orchardServices = services;
            _apiService = apiService;
            _query.Add("format", "json");
            _query.Add("flavor", "arrays");
        }

        public ActionResult Index(int id) {
            var part = _orchardServices.ContentManager.Get(id).As<ConfigurationPart>();
            if (part == null) {
                return new HttpNotFoundResult();
            }

            if (User.Identity.IsAuthenticated) {
                if (!_orchardServices.Authorizer.Authorize(global::Orchard.Core.Contents.Permissions.ViewContent, part)) {
                    return new HttpUnauthorizedResult();
                }
            } else {
                System.Web.Security.FormsAuthentication.RedirectToLoginPage(Request.RawUrl);
            }

            return View(part);
        }

        public ActionResult Load(int id) {
            Response.AddHeader("Access-Control-Allow-Origin", "*");
            ConfigurationPart part;
            ApiRequest request;

            foreach (var rejection in _apiService.Rejections(id, out request, out part)) {
                return rejection.ContentResult(_query["format"].ToString(), _query["flavor"].ToString());
            }

            request.RequestType = ApiRequestType.Execute;

            var root = new TflRoot(part.Configuration, _query);
            var errors = root.Errors();
            if (errors.Any()) {
                request.Status = 500;
                request.Message = string.Join(Environment.NewLine, errors);
                return new ApiResponse(request, root.ToString(), new TransformalizeResponse()).ContentResult(_query["format"], _query["flavor"]);
            }

            TransformConfigurationForLoad(root);

            var transformalizeRequest = new TransformalizeRequest(part, _query, null, Logger, root);

            return Run(request, transformalizeRequest).ContentResult(_query["format"], _query["flavor"]);
        }

        //todo: we don't want it to actually download, we just want a link to the download
        //public FilePathResult Download(int id) {
        //    var response = Run(id, TransformConfigurationForDownload, _query);
        //    var file = response.TransformalizeResponse.Processes[0].OutputConnection.File;
        //    var ext = Path.GetExtension(file) ?? "csv";
        //    return new FilePathResult(file, "application/" + ext.TrimStart(".".ToCharArray())) {
        //        FileDownloadName = file
        //    };
        //}

        public ActionResult Save(int id) {
            Response.AddHeader("Access-Control-Allow-Origin", "*");
            ConfigurationPart part;
            ApiRequest request;

            foreach (var rejection in _apiService.Rejections(id, out request, out part)) {
                return rejection.ContentResult(_query["format"], _query["flavor"]);
            }

            request.RequestType = ApiRequestType.Execute;
            _query.Add("data", Request.Form["data"]);
            var transformalizeRequest = new TransformalizeRequest(part, _query, null, Logger);

            return Run(request, transformalizeRequest).ContentResult(_query["format"], _query["flavor"]);
        }

    }
}