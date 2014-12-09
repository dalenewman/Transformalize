using System.Collections.Specialized;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Themes;
using Transformalize.Orchard.Models;
using Transformalize.Orchard.Services;

namespace Transformalize.Orchard.Controllers {

    [Themed]
    public class HandsOnTableController : WidgetController {

        private readonly IOrchardServices _orchardServices;
        private readonly IApiService _apiService;
        private readonly NameValueCollection _query = new NameValueCollection(2);

        public HandsOnTableController(
            IOrchardServices services,
            ITransformalizeService transformalize,
            IApiService apiService
        )
            : base(services, transformalize, apiService) {
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
                return rejection.ContentResult(_query["format"], _query["flavor"]);
            }

            request.RequestType = ApiRequestType.Execute;
            var modified = TransformConfigurationForLoad(part.Configuration);

            return Run(request, part, _query, modified)
                .ContentResult(_query["format"], _query["flavor"]);
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
            return Run(request, part, _query).ContentResult(_query["format"], _query["flavor"]);
        }

    }
}